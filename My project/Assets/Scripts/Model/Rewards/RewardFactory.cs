using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Controller;
using Model.Tetri;
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

        public List<string> allPossibleCellTypeNames;
        public List<string> allPossibleCharacterTypeNames;
        private List<Type> cachedCellTypes;
        private List<Type> cachedCharacterTypes;

#if UNITY_EDITOR
        [ContextMenu("自动收集所有Cell类型")]
        public void AutoCollectAllCellTypes()
        {
            var cellTypes = Assembly.GetAssembly(typeof(Cell))
                .GetTypes()
                .Where(type => type.IsSubclassOf(typeof(Cell))
                            && !type.IsSubclassOf(typeof(Character))
                            && type != typeof(Empty)
                            && type != typeof(Padding)
                            && !type.IsAbstract)
                .Select(t => t.FullName)
                .ToList();
            allPossibleCellTypeNames = cellTypes;
            EditorUtility.SetDirty(this);
            Debug.Log("已自动收集所有Cell类型到allPossibleCellTypeNames");

            var characterTypes = Assembly.GetAssembly(typeof(Character))
                .GetTypes()
                .Where(type => type.IsSubclassOf(typeof(Character))
                            && type != typeof(Empty)
                            && type != typeof(Padding)
                            && !type.IsAbstract)
                .Select(t => t.FullName)
                .ToList();
            allPossibleCharacterTypeNames = characterTypes;
            EditorUtility.SetDirty(this);
            Debug.Log("已自动收集所有Character类型到allPossibleCharacterTypeNames");
        }
#endif


        [SerializeField] private int rewardCount;
        private TetrisFactory tetrisFactory = new TetrisFactory();
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

            cachedCellTypes = allPossibleCellTypeNames
                .Select(FindType)
                .Where(t => t != null)
                .ToList();

            cachedCharacterTypes = allPossibleCharacterTypeNames
                .Select(FindType)
                .Where(t => t != null)
                .ToList();
        }

        private Type FindType(string typeName)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var type = assembly.GetType(typeName);
                if (type != null)
                    return type;
            }
            return null;
        }

        private bool HasUnownedCellType(TetriInventoryModel inventory)
        {

            var ownedTypes = inventory.CellTypes;
            return cachedCellTypes.Any(type => !ownedTypes.Contains(type));
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
            var characterTypes = allPossibleCharacterTypeNames
                .Select(Type.GetType)
                .Where(t => t != null)
                .ToList();
            // todo inventory有一个方法得到CellTypes
            var ownedTypes = new HashSet<Type>(
                inventory.GetAllTetris()
                    .SelectMany(tetri => tetri.GetOccupiedPositions()
                        .Select(pos => tetri.Shape[pos.x, pos.y].GetType()))
                    .Where(type => typeof(Character).IsAssignableFrom(type))
            );
            return characterTypes.Any(type => !ownedTypes.Contains(type));
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
            var allTypes = cachedCellTypes;

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

            var tetriInstance = tetrisFactory.CreateRandomShapeWithCell(selectedCellType);

            // 7. 返回AddTetri奖励
            return new AddTetri(tetriInstance);
        }

        private Reward CreateNewCharacterReward()
        {
            // 1. 获取所有可能的Character类型
            var allTypes = allPossibleCharacterTypeNames
                .Select(Type.GetType)
                .Where(t => t != null)
                .ToList();

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
            var tetriInstance = tetrisFactory.CreateSingleCellTetri(selectedCharacterType);

            // 6. 返回NewCharacter奖励
            return new NewCharacter(tetriInstance);
        }

        private Reward CreateUpgradeTetriReward()
        {
            // 1. 从 inventory 中随机挑选一个可以升级的 Tetri（含有 Padding 且未升级过）
            var upgradableTetris = tetriInventoryData.GetAllTetris()
                .Where(tetri => tetri.UpgradedTimes < 1)
                .ToList();

            if (upgradableTetris.Count == 0)
            {
                Debug.LogWarning("没有可升级的 Tetri，无法生成 UpgradeTetri 奖励。");
                return null;
            }
            var targetTetri = upgradableTetris[UnityEngine.Random.Range(0, upgradableTetris.Count)];

            return new UpgradeTetri(targetTetri);

        }

        private Reward CreateUpgradeCharacterReward()
        {
            // 1. 找到所有可升级的 CharacterCell
            var upgradableCharacters = tetriInventoryData.GetAllTetris()
                .SelectMany(tetri => tetri.GetOccupiedPositions()
                    .Select(pos => tetri.Shape[pos.x, pos.y]))
                .OfType<Character>()
                .Where(character => character.Level < 3) // todo magic
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
