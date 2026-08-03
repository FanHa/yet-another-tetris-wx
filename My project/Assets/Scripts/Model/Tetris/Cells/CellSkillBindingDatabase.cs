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

        public List<CellSkillBindingItem> GetBindings()
        {
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
