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

        private Dictionary<string, CellSkillBindingItem> bindingByCellId;

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

        public SkillDefinition GetSkillDefinition(string cellId)
        {
            EnsureInitialized();
            CellSkillBindingItem binding = bindingByCellId[cellId];
            return binding.SkillDefinition;
        }

        public SkillConfig GetResolvedSkillConfig(string cellId)
        {
            SkillDefinition skillDefinition = GetSkillDefinition(cellId);
            return skillDefinition.Config;
        }

        public List<CellSkillBindingItem> GetBindings()
        {
            return new List<CellSkillBindingItem>(bindings);
        }

        public List<string> GetBoundCellIds()
        {
            EnsureInitialized();
            return bindingByCellId.Keys.ToList();
        }

        private void RebuildIndex()
        {
            bindingByCellId = new Dictionary<string, CellSkillBindingItem>(StringComparer.Ordinal);

            foreach (CellSkillBindingItem item in bindings)
            {
                if (item == null || string.IsNullOrWhiteSpace(item.CellId))
                {
                    continue;
                }

                bindingByCellId[item.CellId] = item;
            }
        }

        private void EnsureInitialized()
        {
            if (bindingByCellId != null)
            {
                return;
            }

            RebuildIndex();
        }
    }
}
