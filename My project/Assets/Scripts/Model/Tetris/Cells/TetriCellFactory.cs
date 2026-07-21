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
        public IReadOnlyDictionary<Type, CellTypeId> TypeToCellTypeId => cellDatabase != null ? cellDatabase.TypeBridgeToCellTypeId : emptyTypeToCellTypeId;

        public IReadOnlyDictionary<CharacterTypeId, Type> CharacterTypeIdToType { get; private set; }
        public IReadOnlyDictionary<Type, CharacterTypeId> TypeToCharacterTypeId { get; private set; }
        public IReadOnlyDictionary<Type, SkillConfig> CharacterTypeToConfig { get; private set; }

        private static readonly IReadOnlyDictionary<CellTypeId, Type> emptyCellTypeIdToType = new Dictionary<CellTypeId, Type>();
        private static readonly IReadOnlyDictionary<Type, CellTypeId> emptyTypeToCellTypeId = new Dictionary<Type, CellTypeId>();
        private static readonly IReadOnlyDictionary<Type, SkillConfig> emptyCellTypeToConfig = new Dictionary<Type, SkillConfig>();

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

        public List<CellTypeId> GetRegisteredPlayableCellTypeIds()
        {
            return cellDatabase.GetPlayableCellTypeIds();
        }

        public List<string> GetRegisteredPlayableCellIds()
        {
            return cellDatabase.GetPlayableCellIds();
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
            if (!cellDatabase.TryGetCellType(cellTypeId, out var type))
                throw new ArgumentException($"Unknown CellTypeId. {BuildErrorContext(nameof(CreateCell), cellTypeId)}", nameof(cellTypeId));

            cellDatabase.TryGetConfig(cellTypeId, out SkillConfig config);
            return CreateCellFromResolvedType(type, config, $"cell type: {cellTypeId}", nameof(cellTypeId));
        }

        public Cell CreateCell(string cellId)
        {
            if (string.IsNullOrWhiteSpace(cellId))
                throw new ArgumentException($"Cell id is null or empty. {BuildErrorContext(nameof(CreateCell), cellId)}", nameof(cellId));

            if (!cellDatabase.TryGetCellType(cellId, out var type))
                throw new ArgumentException($"Unknown cell id. {BuildErrorContext(nameof(CreateCell), cellId)}", nameof(cellId));

            cellDatabase.TryGetConfig(cellId, out SkillConfig config);
            return CreateCellFromResolvedType(type, config, $"cell id: {cellId}", nameof(cellId));
        }

        private Cell CreateCellFromResolvedType(Type type, SkillConfig config, string debugTarget, string argumentName)
        {
            if (type == null || !typeof(Cell).IsAssignableFrom(type))
                throw new ArgumentException($"Resolved type is not a valid {nameof(Cell)} for {debugTarget}.", argumentName);

            var cell = (Cell)Activator.CreateInstance(type);

            if (config != null)
            {
                cell.Config = config;
            }
            else
            {
                Debug.LogWarning($"No config registered for {debugTarget}.");
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

            if (CharacterConfigMap != null && CharacterConfigMap.TryGetValue(type, out var config) && config != null)
            {
                cell.Config = config;
            }
            else
            {
                Debug.LogWarning($"No config registered for character type: {type.Name}");
            }
            return cell;
        }

        public Cell Clone(Cell cell)
        {
            if (cell == null)
                throw new ArgumentNullException(nameof(cell), $"Source cell is null. {BuildErrorContext(nameof(Clone), CellTypeId.None)}");

            var clone = (Cell)Activator.CreateInstance(cell.GetType());
            clone.Config = cell.Config;
            clone.Level = cell.Level;

            if (cell is Padding sourcePadding && clone is Padding clonePadding)
            {
                clonePadding.Affinity = sourcePadding.Affinity;
            }

            return clone;
        }
    }

}

