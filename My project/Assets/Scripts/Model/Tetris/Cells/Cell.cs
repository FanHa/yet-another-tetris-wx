using System;
using Units.Skills;
using UnityEngine;

namespace Model.Tetri
{
    [Serializable]
    public class Cell
    {
        private int level = 1;
        private SkillBackedCellDefinition definition;
        private SkillDefinition skillDefinition;
        private AffinityType affinity = AffinityType.None;

        public int Level
        {
            get => level;
            set => level = Mathf.Max(1, value);
        }

        // Runtime primary id for the CellDefinition workflow.
        public virtual string CellId => definition?.Id ?? GetType().Name;

        public SkillBackedCellDefinition Definition => definition;
        public SkillDefinition SkillDefinition => skillDefinition;
        public bool IsSkillBacked => skillDefinition != null;

        public SkillConfig Config;

        public virtual string Description() => skillDefinition != null ? skillDefinition.Description : GetType().Name;
        public virtual string Name() => skillDefinition != null ? skillDefinition.DisplayName : GetType().Name;

        public virtual void Initialize(SkillBackedCellDefinition skillCellDefinition)
        {
            definition = skillCellDefinition;
            skillDefinition = skillCellDefinition?.SkillDefinition;
            affinity = skillCellDefinition?.Affinity ?? AffinityType.None;
            Config = skillDefinition?.Config;
        }

        public virtual void Apply(Units.Unit unit) { }
        public virtual AffinityType Affinity
        {
            get => affinity;
            set => affinity = value;
        }
    }
}