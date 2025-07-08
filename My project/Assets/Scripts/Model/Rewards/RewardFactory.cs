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
        // [SerializeField] private CellTypeCatalog cellTypeCatalog;
        private List<RewardTypeConfig> rewardTypeConfigs;
        private List<CellTypeId> availableCellTypeIds;
        private List<CharacterTypeId> availableCharacterTypeIds;


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
            availableCellTypeIds = Enum.GetValues(typeof(CellTypeId))
                .Cast<CellTypeId>()
                .Where(typeId => typeId != CellTypeId.Padding)
                .ToList();
            availableCharacterTypeIds = Enum.GetValues(typeof(CharacterTypeId)).Cast<CharacterTypeId>().ToList();
        }
        private bool HasUnownedCellType(TetriInventoryModel inventory)
        {
            IReadOnlyCollection<CellTypeId> ownedTypeIds = inventory.ExistCellTypeIds;
            return availableCellTypeIds.Any(typeId => !ownedTypeIds.Contains(typeId));
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
            IReadOnlyCollection<CharacterTypeId> ownedCharacterTypeIds = inventory.ExistCharacterTypeIds;
            return availableCharacterTypeIds.Any(type => !ownedCharacterTypeIds.Contains(type));
        }

        private bool HasUpgradeableCharacter(TetriInventoryModel inventory)
        {
            // 检查是否有未满级的 CharacterTypeId
            foreach (var tetri in inventory.GetAllTetris())
            {
                if (tetri.Type == Model.Tetri.Tetri.TetriType.Character)
                {
                    foreach (var pos in tetri.GetOccupiedPositions())
                    {
                        var cell = tetri.Shape[pos.x, pos.y] as Character;
                        if (cell != null && cell.Level < 3)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
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
            var rewards = new List<Reward>();
            var usedKeys = new HashSet<string>();
            int maxTries = rewardCount * 5; // 防止死循环
            int tries = 0;
            while (rewards.Count < rewardCount && tries < maxTries)
            {
                var reward = GenerateReward();
                if (reward == null)
                {
                    tries++;
                    continue;
                }

                // 生成唯一性key
                string key = GetRewardKey(reward);
                if (!usedKeys.Contains(key))
                {
                    rewards.Add(reward);
                    usedKeys.Add(key);
                }
                tries++;
            }
            return rewards;
        }

        private string GetRewardKey(Reward reward)
        {
            switch (reward)
            {
                case AddTetri addTetri:
                    return "AddTetri_" + addTetri.GetTetri().GetMainCell().CellTypeId;
                case NewCharacter newChar:
                    var characterCell = newChar.GetTetri().GetMainCell() as Character;
                    return "NewCharacter_" + characterCell.CharacterTypeId;
                case UpgradeCoreCell upgradeCore:
                    return "UpgradeCoreCell_" + upgradeCore.TargetTetri.GetHashCode();
                case UpgradeNoneCoreCells upgradeNoneCore:
                    return "UpgradeNoneCore_" + upgradeNoneCore.TargetTetri.GetHashCode();
                case UpgradeCharacter upgradeChar:
                    return "UpgradeCharacter_" + upgradeChar.TargetTetri.GetHashCode();
                default:
                    return reward.GetType().Name;
            }
        }

        private Reward CreateNewTetriReward()
        {
            var ownedTypeIds = tetriInventoryData.ExistCellTypeIds;
            var unownedTypeIds = availableCellTypeIds.Where(type => !ownedTypeIds.Contains(type)).ToList();

            if (unownedTypeIds.Count == 0)
            {
                Debug.LogError("没有可用的Cell类型，无法生成NewTetri奖励。");
                return null;
            }

            // 4. 随机选择一个Cell类型
            var selectedCellType = unownedTypeIds[UnityEngine.Random.Range(0, unownedTypeIds.Count)];

            var tetriInstance = tetriModelFactory.CreateRandomShapeWithCell(selectedCellType);

            // 7. 返回AddTetri奖励
            return new AddTetri(tetriInstance);
        }

        private Reward CreateNewCharacterReward()
        {
            IReadOnlyCollection<CharacterTypeId> existCharacterTypeIds = tetriInventoryData.ExistCharacterTypeIds;

            // 3. 找出未拥有的Character类型
            var unownedTypeIds = availableCharacterTypeIds.Where(type => !existCharacterTypeIds.Contains(type)).ToList();

            if (unownedTypeIds.Count == 0)
            {
                Debug.LogError("没有可用的Character类型，无法生成NewCharacter奖励。");
                return null;
            }

            // 4. 随机选择一个Character类型
            var selectedCharacterTypeId = unownedTypeIds[UnityEngine.Random.Range(0, unownedTypeIds.Count)];

            // 5. 创建Character实例
            var tetriInstance = tetriModelFactory.CreateCharacterTetri(selectedCharacterTypeId);

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
                tetri => new UpgradeNoneCoreCells(tetri, tetriModelFactory),
                tetri => new UpgradeCoreCell(tetri)
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
