using System;
using System.Collections.Generic;
using System.Linq;
using Units.Skills;
using UnityEngine;

namespace Model.Tetri
{
    [CreateAssetMenu(fileName = "CellSkillBindingDatabase", menuName = "Config/Cells/Cell Skill Binding Database")]
    public sealed class CellSkillBindingDatabase : ScriptableObject
    {
        [SerializeField] private List<CellSkillBindingItem> bindings = new();

        private Dictionary<string, SkillConfig> skillConfigByCellId;

        public IReadOnlyList<CellSkillBindingItem> Bindings => bindings;

        private void OnEnable()
        {
            RebuildIndex();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            RebuildIndex();
        }
#endif

        public bool TryGetSkillConfig(string cellId, out SkillConfig skillConfig)
        {
            EnsureInitialized();
            skillConfig = null;

            if (string.IsNullOrWhiteSpace(cellId))
            {
                return false;
            }

            return skillConfigByCellId.TryGetValue(cellId, out skillConfig);
        }

        public bool TryGetSkillConfig(CellDefinition definition, out SkillConfig skillConfig)
        {
            skillConfig = null;
            return definition != null && TryGetSkillConfig(definition.Id, out skillConfig);
        }

        public bool TryGetSkillConfig(Type cellType, CellDatabase cellDatabase, out SkillConfig skillConfig)
        {
            skillConfig = null;

            if (cellType == null || cellDatabase == null)
            {
                return false;
            }

            if (!cellDatabase.TryGetId(cellType, out string cellId) || string.IsNullOrWhiteSpace(cellId))
            {
                return false;
            }

            return TryGetSkillConfig(cellId, out skillConfig);
        }

        public List<CellSkillBindingItem> GetBindings()
        {
            if (bindings == null)
            {
                return new List<CellSkillBindingItem>();
            }

            return new List<CellSkillBindingItem>(bindings);
        }

        public List<string> GetBoundCellIds()
        {
            EnsureInitialized();
            return skillConfigByCellId.Keys.ToList();
        }

        private void RebuildIndex()
        {
            skillConfigByCellId = new Dictionary<string, SkillConfig>(StringComparer.Ordinal);

            if (bindings == null)
            {
                return;
            }

            foreach (CellSkillBindingItem item in bindings)
            {
                if (item == null || string.IsNullOrWhiteSpace(item.CellId))
                {
                    continue;
                }

                skillConfigByCellId[item.CellId] = item.SkillConfig;
            }
        }

        private void EnsureInitialized()
        {
            if (skillConfigByCellId != null)
            {
                return;
            }

            RebuildIndex();
        }
    }
}
