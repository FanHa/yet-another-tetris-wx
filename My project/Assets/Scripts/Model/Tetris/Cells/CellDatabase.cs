using System;
using System.Collections.Generic;
using System.Linq;
using Units.Skills;
using UnityEngine;

namespace Model.Tetri
{
    [CreateAssetMenu(fileName = "CellDatabase", menuName = "Config/Cells/Cell Database")]
    public sealed class CellDatabase : ScriptableObject
    {
        [SerializeField] private List<CellDefinition> definitions = new();

        private Dictionary<string, CellDefinition> definitionById;
        private Dictionary<string, Type> cellTypeById;
        private Dictionary<Type, string> idByCellType;
        private Dictionary<string, SkillConfig> configById;

        private Dictionary<CellTypeId, Type> bridgeTypeByCellTypeId;
        private Dictionary<Type, CellTypeId> bridgeCellTypeIdByType;
        private Dictionary<Type, SkillConfig> bridgeConfigByType;

        public IReadOnlyList<CellDefinition> Definitions => definitions;
        public IReadOnlyDictionary<CellTypeId, Type> CellTypeIdBridgeToType => bridgeTypeByCellTypeId;
        public IReadOnlyDictionary<Type, CellTypeId> TypeBridgeToCellTypeId => bridgeCellTypeIdByType;
        public IReadOnlyDictionary<Type, SkillConfig> CellTypeBridgeToConfig => bridgeConfigByType;

        private void OnEnable()
        {
            InitializeIndexes();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            InitializeIndexes();
        }
#endif

        private void InitializeIndexes()
        {
            definitionById = new Dictionary<string, CellDefinition>(StringComparer.Ordinal);
            cellTypeById = new Dictionary<string, Type>(StringComparer.Ordinal);
            idByCellType = new Dictionary<Type, string>();
            configById = new Dictionary<string, SkillConfig>(StringComparer.Ordinal);

            bridgeTypeByCellTypeId = new Dictionary<CellTypeId, Type>();
            bridgeCellTypeIdByType = new Dictionary<Type, CellTypeId>();
            bridgeConfigByType = new Dictionary<Type, SkillConfig>();

            if (definitions == null)
            {
                return;
            }

            foreach (CellDefinition definition in definitions)
            {
                if (definition == null || string.IsNullOrWhiteSpace(definition.Id))
                {
                    continue;
                }

                definitionById[definition.Id] = definition;
                configById[definition.Id] = definition.Config;

                if (!definition.TryResolveCellType(out Type cellType, out _))
                {
                    continue;
                }

                cellTypeById[definition.Id] = cellType;
                idByCellType[cellType] = definition.Id;

                if (Activator.CreateInstance(cellType) is Cell cell)
                {
                    bridgeTypeByCellTypeId[cell.CellTypeId] = cellType;
                    bridgeCellTypeIdByType[cellType] = cell.CellTypeId;
                    bridgeConfigByType[cellType] = definition.Config;
                }
            }
        }

        public bool TryGetDefinition(string id, out CellDefinition definition)
        {
            EnsureInitialized();
            return definitionById.TryGetValue(id, out definition);
        }

        public bool TryGetCellType(string id, out Type cellType)
        {
            EnsureInitialized();
            return cellTypeById.TryGetValue(id, out cellType);
        }

        public bool TryGetCellType(CellTypeId cellTypeId, out Type cellType)
        {
            EnsureInitialized();
            return bridgeTypeByCellTypeId.TryGetValue(cellTypeId, out cellType);
        }

        public bool TryGetConfig(string id, out SkillConfig config)
        {
            EnsureInitialized();
            return configById.TryGetValue(id, out config);
        }

        public bool TryGetConfig(CellTypeId cellTypeId, out SkillConfig config)
        {
            EnsureInitialized();
            config = null;

            if (!bridgeTypeByCellTypeId.TryGetValue(cellTypeId, out Type cellType))
            {
                return false;
            }

            return bridgeConfigByType.TryGetValue(cellType, out config);
        }

        public bool TryGetId(Type cellType, out string id)
        {
            EnsureInitialized();

            if (cellType == null)
            {
                id = null;
                return false;
            }

            return idByCellType.TryGetValue(cellType, out id);
        }

        public List<CellDefinition> GetDefinitions()
        {
            if (definitions == null)
            {
                return new List<CellDefinition>();
            }

            return new List<CellDefinition>(definitions);
        }

        public List<string> GetPlayableCellIds()
        {
            EnsureInitialized();
            return definitionById.Keys
                .Where(id => !string.IsNullOrWhiteSpace(id))
                .ToList();
        }

        public List<CellTypeId> GetPlayableCellTypeIds()
        {
            EnsureInitialized();
            return bridgeTypeByCellTypeId.Keys
                .Where(id => id != CellTypeId.Padding && id != CellTypeId.None)
                .ToList();
        }

        private void EnsureInitialized()
        {
            if (definitionById != null && cellTypeById != null && idByCellType != null)
            {
                return;
            }

            InitializeIndexes();
        }
    }
}