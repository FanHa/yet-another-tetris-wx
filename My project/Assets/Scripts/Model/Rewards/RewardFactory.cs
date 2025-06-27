using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Controller;
using Model.Tetri;
using Operation;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace Model.Rewards
{
    [CreateAssetMenu(menuName = "Config/RewardFactory")]
    public class RewardFactory : ScriptableObject
    {

        public enum RewardType { NewTetri, NewCharacter, UpgradeTetri, UpgradeCharacter }
        public class RewardTypeConfig
        {
            public RewardType type;
            public float probability;
            public Func<TetriInventoryModel, bool> isAvailable;
        }

        [SerializeField] private int rewardCount;
        [SerializeField] private Model.Tetri.TetriFactory tetriModelFactory;
        [SerializeField] private Model.Tetri.TetriCellFactory tetriCellFactory;

        [SerializeField] private Model.TetriInventoryModel tetriInventoryData;
        private  List<RewardTypeConfig> rewardTypeConfigs;


        public void OnEnable()
        {
            rewardTypeConfigs = new List<RewardTypeConfig>
            {
                new RewardTypeConfig
                {
                    type = RewardType.NewTetri,
                    probability = 0.5f,
                    isAvailable = HasUnownedCellType
                },
                new RewardTypeConfig
                {
                    type = RewardType.UpgradeTetri,
                    probability = 0.3f,
                    isAvailable = HasEnoughUpgradeableTetri
                },
                new RewardTypeConfig
                {
                    type = RewardType.NewCharacter,
                    probability = 0.2f,
                    isAvailable = HasUnownedCharacterCell
                },
                new RewardTypeConfig
                {
                    type = RewardType.UpgradeCharacter,
                    probability = 0.2f, // 可调整
                    isAvailable = HasUpgradeableCharacter
                },
            };

        }
        private bool HasUnownedCellType(TetriInventoryModel inventory)
        {

            var ownedTypes = inventory.CellTypes;
            return tetriCellFactory.AvailableCellTypes.Any(type => !ownedTypes.Contains(type));
        }

        private bool HasEnoughUpgradeableTetri(Model.TetriInventoryModel inventory)
        {
            List<Model.Tetri.Tetri> allTetris = inventory.GetAllTetris();
            int total = allTetris.Count;
            if (total == 0) return false;

            int unupgradedCount = allTetris.Count(tetri =>
                tetri.UpgradedTimes < 1
            );

            return unupgradedCount > 4 && (unupgradedCount / (float)total) > 0.5f;
        }

        public bool HasUnownedCharacterCell(TetriInventoryModel inventory)
        {
            // todo inventory有一个方法得到CellTypes
            var ownedTypes = new HashSet<Type>(
                inventory.GetAllTetris()
                    .SelectMany(tetri => tetri.GetOccupiedPositions()
                        .Select(pos => tetri.Shape[pos.x, pos.y].GetType()))
                    .Where(type => typeof(Character).IsAssignableFrom(type))
            );
            return tetriCellFactory.AvailableCharacterTypes.Any(type => !ownedTypes.Contains(type));
        }

        private bool HasUpgradeableCharacter(TetriInventoryModel inventory)
        {
            // 假设 Character 有 Level 属性，且未满级可升级
            return inventory.CellTypes
                .Where(type => typeof(Character).IsAssignableFrom(type))
                .Any(type =>
                {
                    // 检查 inventory 里是否有该类型的 CharacterCell 未满级
                    return inventory.GetAllTetris()
                        .SelectMany(tetri => tetri.GetOccupiedPositions()
                            .Select(pos => tetri.Shape[pos.x, pos.y]))
                        .OfType<Character>()
                        .Any(cell => cell.GetType() == type && cell.Level < 3); // todo
                });
        }


        public Reward GenerateReward()
        {
            var availableConfigs = rewardTypeConfigs
                .Where(cfg => cfg.isAvailable == null || cfg.isAvailable(tetriInventoryData))
                .ToList();
            if (availableConfigs.Count == 0)
                return null;

            float total = availableConfigs.Sum(cfg => cfg.probability);
            float rand = UnityEngine.Random.value * total;
            float acc = 0f;
            RewardTypeConfig selected = null;
            foreach (var cfg in availableConfigs)
            {
                acc += cfg.probability;
                if (rand <= acc)
                {
                    selected = cfg;
                    break;
                }
            }
            if (selected == null)
                selected = availableConfigs.Last();

            // 生成具体奖励对象
            switch (selected.type)
            {
                case RewardType.NewTetri: return CreateNewTetriReward();
                case RewardType.NewCharacter: return CreateNewCharacterReward();
                case RewardType.UpgradeTetri: return CreateUpgradeTetriReward();
                case RewardType.UpgradeCharacter: return CreateUpgradeCharacterReward();

                default: return null;
            }
        }


        public List<Reward> GenerateRewards()
        {
            List<Reward> rewards = new List<Reward>();
            for (int i = 0; i < rewardCount; i++)
            {
                var reward = GenerateReward();
                if (reward != null)
                    rewards.Add(reward);
            }
            return rewards;
        }

        private Reward CreateNewTetriReward()
        {
            // 1. 获取所有可能的Cell类型
            List<Type> allTypes = tetriCellFactory.AvailableCellTypes;

            // 2. 获取inventory中已有的Cell类型
            var ownedTypes = tetriInventoryData.CellTypes;

            // 3. 找出未拥有的Cell类型
            var unownedTypes = allTypes.Where(type => !ownedTypes.Contains(type)).ToList();

            if (unownedTypes.Count == 0)
            {
                // 如果全部拥有，降级为随机已有类型
                // todo 不该发生,需要日志
                unownedTypes = allTypes;
            }

            // 4. 随机选择一个Cell类型
            var selectedCellType = unownedTypes[UnityEngine.Random.Range(0, unownedTypes.Count)];

            var tetriInstance = tetriModelFactory.CreateRandomShapeWithCell(selectedCellType);

            // 7. 返回AddTetri奖励
            return new AddTetri(tetriInstance);
        }

        private Reward CreateNewCharacterReward()
        {
            // 1. 获取所有可能的Character类型
            List<Type> allTypes = tetriCellFactory.AvailableCharacterTypes;

            // 2. 获取inventory中已有的Character类型
            var ownedTypes = tetriInventoryData.CellTypes
                .Where(type => typeof(Character).IsAssignableFrom(type))
                .ToList();

            // 3. 找出未拥有的Character类型
            var unownedTypes = allTypes.Where(type => !ownedTypes.Contains(type)).ToList();

            if (unownedTypes.Count == 0)
            {
                // 如果全部拥有，降级为随机已有类型
                // todo 不该发生,需要日志
                unownedTypes = allTypes;
            }

            // 4. 随机选择一个Character类型
            var selectedCharacterType = unownedTypes[UnityEngine.Random.Range(0, unownedTypes.Count)];

            // 5. 创建Character实例
            var tetriInstance = tetriModelFactory.CreateSingleCellTetri(selectedCharacterType);

            // 6. 返回NewCharacter奖励
            return new NewCharacter(tetriInstance);
        }

        private Reward CreateUpgradeTetriReward()
        {
            // 1. 从 inventory 中随机挑选一个可以升级的 Tetri（含有 Padding 且未升级过）
            List<Model.Tetri.Tetri> upgradableTetris = tetriInventoryData.GetAllTetris()
                .Where(tetri => tetri.Type is Model.Tetri.Tetri.TetriType.Normal && tetri.CanBeUpgraded())
                .ToList();
            if (upgradableTetris.Count == 0)
            {
                Debug.LogWarning("没有可升级的 Tetri，无法生成 UpgradeTetri 奖励。");
                return null;
            }
            Model.Tetri.Tetri targetTetri = upgradableTetris[UnityEngine.Random.Range(0, upgradableTetris.Count)];

            Func<Model.Tetri.Tetri, Reward>[] upgradeFactories = new Func<Model.Tetri.Tetri, Reward>[]
            {
                tetri => new UpgradeNoneCoreCellsTetri(tetri),
                tetri => new UpgradeCoreCellTetri(tetri)
            };
            // 随机选择一个 UpgradeTetri 的衍生类
            var selectedFactory = upgradeFactories[UnityEngine.Random.Range(0, upgradeFactories.Length)];
            return selectedFactory(targetTetri);

        }

        private Reward CreateUpgradeCharacterReward()
        {
            // 1. 找到所有可升级的 CharacterCell
            List<Model.Tetri.Tetri> upgradableCharacters = tetriInventoryData.GetAllTetris()
                .Where(tetri => tetri.Type == Model.Tetri.Tetri.TetriType.Character)
                .Where(tetri => tetri.CanBeUpgraded())
                .ToList();

            if (upgradableCharacters.Count == 0)
            {
                Debug.LogWarning("没有可升级的角色，无法生成 UpgradeCharacter 奖励。");
                return null;
            }

            // 2. 随机选择一个可升级的角色
            var targetCharacter = upgradableCharacters[UnityEngine.Random.Range(0, upgradableCharacters.Count)];

            // 3. 返回 UpgradeCharacter 奖励
            return new UpgradeCharacter(targetCharacter);
        }
    }
}
