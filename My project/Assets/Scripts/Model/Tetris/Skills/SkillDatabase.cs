using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Model.Tetri
{
    [CreateAssetMenu(menuName = "Config/Skills/Skill Database")]
    public sealed class SkillDatabase : ScriptableObject
    {
        [SerializeField] private List<SkillDefinition> definitions = new();

        public IReadOnlyList<SkillDefinition> Definitions => definitions;

        public SkillDefinition GetDefinition(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return null;
            }

            return definitions?.FirstOrDefault(definition =>
                definition != null && string.Equals(definition.Id, id, StringComparison.Ordinal));
        }

        public IReadOnlyList<SkillDefinition> GetDefinitions()
        {
            return definitions?
                .Where(definition => definition != null)
                .ToList()
                ?? new List<SkillDefinition>();
        }
    }
}
