using System;
using System.Collections.Generic;
using System.Linq;
using Units.Skills;
using UnityEngine;

namespace Model.Tetri
{
    public class CharacterTypeMeta
    {
        public CharacterTypeId id;
        public Type type;
        public SkillConfig config;

        public CharacterTypeMeta(CharacterTypeId id, Type type, SkillConfig config)
        {
            this.id = id;
            this.type = type;
            this.config = config;
        }
    }

    [CreateAssetMenu(menuName = "Factory/TetriCellModelFactory")]
    public class TetriCellFactory : ScriptableObject
    {
        public IReadOnlyDictionary<CharacterTypeId, Type> CharacterTypeIdToType { get; private set; }
        public IReadOnlyDictionary<Type, CharacterTypeId> TypeToCharacterTypeId { get; private set; }
        public IReadOnlyDictionary<Type, SkillConfig> CharacterTypeToConfig { get; private set; }

        private IReadOnlyDictionary<CharacterTypeId, Type> CharacterTypeIdMap
        {
            get
            {
                EnsureInitialized();
                return CharacterTypeIdToType;
            }
        }

        private IReadOnlyDictionary<Type, SkillConfig> CharacterConfigMap
        {
            get
            {
                EnsureInitialized();
                return CharacterTypeToConfig;
            }
        }

        [SerializeField] private CharacterConfigRegistry characterConfigRegistry;
        [SerializeField] private CellDatabase cellDatabase;

        private void OnEnable()
        {
            EnsureInitialized();
        }

        private void EnsureInitialized()
        {
            if (CharacterTypeIdToType != null)
            {
                return;
            }

            BuildTypeMaps();
        }

        private void BuildTypeMaps()
        {
            // 角色类型映射表
            var characterTypeMetas = new List<CharacterTypeMeta>
            {
                new(CharacterTypeId.Square, typeof(Square), characterConfigRegistry.SquareCharacterBaseStatConfig),
                new(CharacterTypeId.Triangle, typeof(Triangle), characterConfigRegistry.TriangleCharacterBaseStatConfig),
                new(CharacterTypeId.Circle, typeof(Circle), characterConfigRegistry.CircleCharacterBaseStatConfig),
                new(CharacterTypeId.Aim, typeof(Aim), characterConfigRegistry.AimCharacterBaseStatConfig),
                new(CharacterTypeId.Hourglass, typeof(Hourglass), characterConfigRegistry.HourglassCharacterBaseStatConfig)
            };
            CharacterTypeIdToType = characterTypeMetas.ToDictionary(m => m.id, m => m.type);
            TypeToCharacterTypeId = characterTypeMetas.ToDictionary(m => m.type, m => m.id);
            CharacterTypeToConfig = characterTypeMetas.ToDictionary(m => m.type, m => m.config);

        }

        private static string BuildErrorContext(string entry, Enum id, Type resolvedType = null, Cell sourceCell = null)
        {
            string typeName = resolvedType?.FullName ?? "<null>";
            string sourceTypeName = sourceCell?.GetType().FullName ?? "<null>";
            int sourceLevel = sourceCell?.Level ?? -1;
            return $"[{nameof(TetriCellFactory)}.{entry}] Id={id}, ResolvedType={typeName}, SourceCellType={sourceTypeName}, SourceLevel={sourceLevel}";
        }

        private static string BuildErrorContext(string entry, string id, Type resolvedType = null)
        {
            string typeName = resolvedType?.FullName ?? "<null>";
            return $"[{nameof(TetriCellFactory)}.{entry}] Id={id ?? "<null>"}, ResolvedType={typeName}";
        }



        public Padding CreatePadding()
        {
            return new Padding();
        }

        public List<CharacterTypeId> GetRegisteredCharacterTypeIds()
        {
            if (CharacterTypeIdMap == null)
            {
                return new List<CharacterTypeId>();
            }

            return CharacterTypeIdMap.Keys.ToList();
        }


        public Cell CreateCell(string cellId)
        {
            var definition = cellDatabase.GetDefinition(cellId);
            var type = definition.RuntimeType;
            return CreateCellFromResolvedType(type, null, definition);
        }

        private Cell CreateCellFromResolvedType(Type type, SkillConfig config, CellDefinition definition = null)
        {
            var cell = (Cell)Activator.CreateInstance(type);

            if (definition is SkillBackedCellDefinition skillBackedDefinition)
            {
                cell.Initialize(skillBackedDefinition);
            }
            else if (config != null)
            {
                cell.Config = config;
            }

            return cell;
        }
        
        
        public Character CreateCharacterCell(CharacterTypeId characterTypeId)
        {
            if (!CharacterTypeIdMap.TryGetValue(characterTypeId, out var type))
                throw new ArgumentException($"Unknown CharacterTypeId. {BuildErrorContext(nameof(CreateCharacterCell), characterTypeId)}", nameof(characterTypeId));

            if (type == null || !typeof(Character).IsAssignableFrom(type))
                throw new ArgumentException($"Resolved type is not a valid {nameof(Character)}. {BuildErrorContext(nameof(CreateCharacterCell), characterTypeId, type)}", nameof(characterTypeId));

            // 1. 创建Cell实例（无参构造）
            var cell = (Character)Activator.CreateInstance(type);

            cell.Config = CharacterConfigMap[type] ?? throw new InvalidOperationException(
                $"Character config is null. {BuildErrorContext(nameof(CreateCharacterCell), characterTypeId, type)}");

            return cell;
        }

        public Cell Clone(Cell cell)
        {
            if (cell == null)
                throw new ArgumentNullException(nameof(cell), $"Source cell is null. {BuildErrorContext(nameof(Clone), CellTypeId.None)}");

            var clone = (Cell)Activator.CreateInstance(cell.GetType());
            if (cell.Definition != null)
            {
                clone.Initialize(cell.Definition);
            }
            else
            {
                clone.Config = cell.Config;
            }
            clone.Level = cell.Level;

            if (cell is Padding sourcePadding && clone is Padding clonePadding)
            {
                clonePadding.Affinity = sourcePadding.Affinity;
            }

            return clone;
        }
    }

}

