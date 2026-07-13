using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Units.Skills;
using UnityEditor;
using UnityEngine;

namespace Model.Tetri
{
    public class CellTypeMeta
    {
        public CellTypeId id;
        public Type type;
        public SkillConfig skillConfig;

        public CellTypeMeta(CellTypeId id, Type type, SkillConfig skillConfig)
        {
            this.id = id;
            this.type = type;
            this.skillConfig = skillConfig;
        }
    }

    [CreateAssetMenu(menuName = "Factory/TetriCellModelFactory")]
    public class TetriCellFactory : ScriptableObject
    {

        public IReadOnlyDictionary<CellTypeId, Type> CellTypeIdToType { get; private set; }
        public IReadOnlyDictionary<Type, CellTypeId> TypeToCellTypeId { get; private set; }
        public IReadOnlyDictionary<Type, SkillConfig> CellTypeToConfig { get; private set; }

        public IReadOnlyDictionary<CharacterTypeId, Type> CharacterTypeIdToType { get; private set; }
        public IReadOnlyDictionary<Type, CharacterTypeId> TypeToCharacterTypeId { get; private set; }

        private IReadOnlyDictionary<CellTypeId, Type> CellTypeIdMap
        {
            get
            {
                EnsureInitialized();
                return CellTypeIdToType;
            }
        }

        private IReadOnlyDictionary<Type, SkillConfig> CellConfigMap
        {
            get
            {
                EnsureInitialized();
                return CellTypeToConfig;
            }
        }

        private IReadOnlyDictionary<CharacterTypeId, Type> CharacterTypeIdMap
        {
            get
            {
                EnsureInitialized();
                return CharacterTypeIdToType;
            }
        }

        [SerializeField] private CellLevelConfigManager cellLevelConfigManager;

        private void OnEnable()
        {
            EnsureInitialized();
        }

        private void EnsureInitialized()
        {
            if (CellTypeIdToType != null && CharacterTypeIdToType != null)
            {
                return;
            }

            BuildTypeMaps();
        }

        private void BuildTypeMaps()
        {
            // 只在这里维护一份
            var cellTypeMetas = new List<CellTypeMeta>
            {
                new(CellTypeId.FrostZone, typeof(FrostZone), cellLevelConfigManager.FrostZoneSkillConfig),
                new(CellTypeId.IceShield, typeof(IceShield), cellLevelConfigManager.IceShieldSkillConfig),
                new(CellTypeId.IcyCage, typeof(IcyCage), cellLevelConfigManager.IcyCageSkillConfig),
                new(CellTypeId.Snowball, typeof(Snowball), cellLevelConfigManager.SnowballSkillConfig),
                new(CellTypeId.IceBreaker, typeof(IceBreaker), cellLevelConfigManager.IceBreakerSkillConfig),
                new(CellTypeId.Fireball, typeof(Fireball), cellLevelConfigManager.FireballSkillConfig),
                new(CellTypeId.FlameInject, typeof(FlameInject), cellLevelConfigManager.FlameInjectSkillConfig),
                new(CellTypeId.BlazingField, typeof(BlazingField), cellLevelConfigManager.BlazingFieldSkillConfig),
                new(CellTypeId.FlameRing, typeof(FlameRing), cellLevelConfigManager.FlameRingSkillConfig),
                new(CellTypeId.WindShift, typeof(WindShift), cellLevelConfigManager.WindShiftSkillConfig),
                new(CellTypeId.WildWind, typeof(WildWind), cellLevelConfigManager.WildWindSkillConfig),
                new(CellTypeId.AttackBoost, typeof(AttackBoost), cellLevelConfigManager.AttackBoostSkillConfig),
                new(CellTypeId.LifeBomb, typeof(LifeBomb), cellLevelConfigManager.LifeBombSkillConfig),
                new(CellTypeId.LifeShield, typeof(LifeShield), cellLevelConfigManager.LifeShieldSkillConfig),
                new(CellTypeId.LifePower, typeof(LifePower), cellLevelConfigManager.LifePowerSkillConfig),
                new(CellTypeId.LifeEcho, typeof(LifeEcho), cellLevelConfigManager.LifeEchoSkillConfig),
                new(CellTypeId.ShadowAttack, typeof(ShadowAttack), cellLevelConfigManager.ShadowAttackSkillConfig),
                new(CellTypeId.ShadowStep, typeof(ShadowStep), cellLevelConfigManager.ShadowStepSkillConfig),
                new(CellTypeId.ShadowArrow, typeof(ShadowArrow), cellLevelConfigManager.ShadowArrowSkillConfig),
                new(CellTypeId.VulnerabilityField, typeof(VulnerabilityField), cellLevelConfigManager.VulnerabilityFieldSkillConfig),
                new(CellTypeId.Charge, typeof(Charge), cellLevelConfigManager.ChargeSkillConfig),
                new(CellTypeId.EnergyAbsorb, typeof(EnergyAbsorb), cellLevelConfigManager.EnergyAbsorbSkillConfig),
                new(CellTypeId.ThunderStrike, typeof(Model.Tetri.ThunderStrike), cellLevelConfigManager.ThunderStrikeSkillConfig),
                new(CellTypeId.GuardAlly, typeof(GuardAlly), cellLevelConfigManager.GuardAllySkillConfig),
                new(CellTypeId.WindKnockback, typeof(WindKnockback), cellLevelConfigManager.WindKnockbackSkillConfig),
                
                new(CellTypeId.Padding, typeof(Padding), null) // Padding 不需要配置
            };

            // 自动生成映射表
            CellTypeIdToType = cellTypeMetas.ToDictionary(m => m.id, m => m.type);
            TypeToCellTypeId = cellTypeMetas.ToDictionary(m => m.type, m => m.id);
            CellTypeToConfig = cellTypeMetas.ToDictionary(m => m.type, m => m.skillConfig);

            // 角色类型映射表
            var characterTypeMetas = new List<(CharacterTypeId id, Type type)>
            {
                (CharacterTypeId.Square, typeof(Square)),
                (CharacterTypeId.Triangle, typeof(Triangle)),
                (CharacterTypeId.Circle, typeof(Circle)),
                (CharacterTypeId.Aim, typeof(Aim)),
                (CharacterTypeId.Hourglass, typeof(Hourglass))
            };
            CharacterTypeIdToType = characterTypeMetas.ToDictionary(m => m.id, m => m.type);
            TypeToCharacterTypeId = characterTypeMetas.ToDictionary(m => m.type, m => m.id);

        }



        public Padding CreatePadding()
        {
            return new Padding();
        }

        public List<CellTypeId> GetRegisteredPlayableCellTypeIds()
        {
            if (CellTypeIdMap == null)
            {
                return new List<CellTypeId>();
            }

            return CellTypeIdMap.Keys
                .Where(id => id != CellTypeId.Padding && id != CellTypeId.None)
                .ToList();
        }

        public List<CharacterTypeId> GetRegisteredCharacterTypeIds()
        {
            if (CharacterTypeIdMap == null)
            {
                return new List<CharacterTypeId>();
            }

            return CharacterTypeIdMap.Keys.ToList();
        }


        public Cell CreateCell(CellTypeId cellTypeId)
        {
            if (!CellTypeIdMap.TryGetValue(cellTypeId, out var type))
                throw new ArgumentException($"Unknown CellTypeId: {cellTypeId}");

            if (type == null || !typeof(Cell).IsAssignableFrom(type))
                throw new ArgumentException("Invalid cell type provided.");

            // 1. 创建Cell实例（无参构造）
            var cell = (Cell)Activator.CreateInstance(type);

            // 2. 查找并注入配置
            if (CellConfigMap != null && CellConfigMap.TryGetValue(type, out var config) && config != null)
            {
                cell.SkillConfig = config;
            }
            else
            {
                Debug.LogWarning($"No config registered for cell type: {type.Name}");
            }
            return cell;
        }
        
        
        public Character CreateCharacterCell(CharacterTypeId characterTypeId)
        {
            if (!CharacterTypeIdMap.TryGetValue(characterTypeId, out var type))
                throw new ArgumentException($"Unknown CharacterTypeId: {characterTypeId}");

            if (type == null || !typeof(Character).IsAssignableFrom(type))
                throw new ArgumentException("Invalid character type provided.");

            // 1. 创建Cell实例（无参构造）
            var cell = (Character)Activator.CreateInstance(type);

            // todo character 似乎没有Config
            if (CellConfigMap != null && CellConfigMap.TryGetValue(type, out var config) && config != null)
            {
                cell.SkillConfig = config;
            }
            return cell;
        }

        public Cell Clone(Cell cell)
        {

            var clone = (Cell)Activator.CreateInstance(cell.GetType());
            clone.SkillConfig = cell.SkillConfig; // 复制技能配置
            clone.Level = cell.Level;
            return clone;
        }
    }

}
