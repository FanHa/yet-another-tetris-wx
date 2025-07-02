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
        public SkillConfigGroup configGroup;

        public CellTypeMeta(CellTypeId id, Type type, SkillConfigGroup configGroup)
        {
            this.id = id;
            this.type = type;
            this.configGroup = configGroup;
        }
    }

    [CreateAssetMenu(menuName = "Factory/TetriCellModelFactory")]
    public class TetriCellFactory : ScriptableObject
    {

        public IReadOnlyDictionary<CellTypeId, Type> CellTypeIdToType { get; private set; }
        public IReadOnlyDictionary<Type, CellTypeId> TypeToCellTypeId { get; private set; }
        public IReadOnlyDictionary<Type, SkillConfigGroup> CellTypeToConfig { get; private set; }

        public IReadOnlyDictionary<CharacterTypeId, Type> CharacterTypeIdToType { get; private set; }
        public IReadOnlyDictionary<Type, CharacterTypeId> TypeToCharacterTypeId { get; private set; }

        [SerializeField] private CellLevelConfigManager cellLevelConfigManager;

        private void OnEnable()
        {
            // 只在这里维护一份
            var cellTypeMetas = new List<CellTypeMeta>
            {
                new(CellTypeId.FrostZone, typeof(FrostZone), cellLevelConfigManager.FrostZoneConfigGroup),
                new(CellTypeId.IceShield, typeof(IceShield), cellLevelConfigManager.IceShieldConfigGroup),
                new(CellTypeId.IcyCage, typeof(IcyCage), cellLevelConfigManager.IcyCageConfigGroup),
                new(CellTypeId.Snowball, typeof(Snowball), cellLevelConfigManager.SnowballConfigGroup),
                new(CellTypeId.Fireball, typeof(Fireball), cellLevelConfigManager.FireballConfigGroup),
                new(CellTypeId.FlameInject, typeof(FlameInject), cellLevelConfigManager.FlameInjectConfigGroup),
                new(CellTypeId.BlazingField, typeof(BlazingField), cellLevelConfigManager.BlazingFieldConfigGroup),
                new(CellTypeId.FlameRing, typeof(FlameRing), cellLevelConfigManager.FlameRingConfigGroup),
                new(CellTypeId.Padding, typeof(Padding), null) // Padding 不需要配置
            };

            // 自动生成映射表
            CellTypeIdToType = cellTypeMetas.ToDictionary(m => m.id, m => m.type);
            TypeToCellTypeId = cellTypeMetas.ToDictionary(m => m.type, m => m.id);
            CellTypeToConfig = cellTypeMetas.ToDictionary(m => m.type, m => m.configGroup);

            // 角色类型映射表
            var characterTypeMetas = new List<(CharacterTypeId id, Type type)>
            {
                (CharacterTypeId.Square, typeof(Square)),
                (CharacterTypeId.Triangle, typeof(Triangle)),
                (CharacterTypeId.Circle, typeof(Circle)),
                (CharacterTypeId.Aim, typeof(Aim))
            };
            CharacterTypeIdToType = characterTypeMetas.ToDictionary(m => m.id, m => m.type);
            TypeToCharacterTypeId = characterTypeMetas.ToDictionary(m => m.type, m => m.id);

        }



        public Cell CreatePadding()
        {
            return new Padding();
        }


        public Cell CreateCell(CellTypeId cellTypeId)
        {
            if (!CellTypeIdToType.TryGetValue(cellTypeId, out var type))
                throw new ArgumentException($"Unknown CellTypeId: {cellTypeId}");

            if (type == null || !typeof(Cell).IsAssignableFrom(type))
                throw new ArgumentException("Invalid cell type provided.");

            // 1. 创建Cell实例（无参构造）
            var cell = (Cell)Activator.CreateInstance(type);

            // 2. 查找并注入配置
            if (CellTypeToConfig != null && CellTypeToConfig.TryGetValue(type, out var config) && config != null)
            {
                cell.SetLevelConfig(config);
            }
            else
            {
                Debug.LogWarning($"No config registered for cell type: {type.Name}");
            }
            return cell;
        }
        
        
        public Character CreateCharacterCell(CharacterTypeId characterTypeId)
        {
            if (!CharacterTypeIdToType.TryGetValue(characterTypeId, out var type))
                throw new ArgumentException($"Unknown CharacterTypeId: {characterTypeId}");

            if (type == null || !typeof(Character).IsAssignableFrom(type))
                throw new ArgumentException("Invalid character type provided.");

            // 1. 创建Cell实例（无参构造）
            var cell = (Character)Activator.CreateInstance(type);

            // todo character 似乎没有Config
            if (CellTypeToConfig != null && CellTypeToConfig.TryGetValue(type, out var config) && config != null)
            {
                cell.SetLevelConfig(config);
            }
            return cell;
        }
    }

}
