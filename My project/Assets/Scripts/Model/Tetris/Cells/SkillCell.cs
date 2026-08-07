using System;
using Units;

namespace Model.Tetri
{
    [Serializable]
    public sealed class SkillCell : Cell
    {
        private SkillBackedCellDefinition definition;
        private SkillDefinition skillDefinition;
        private AffinityType affinity = AffinityType.None;

        public override string CellId => definition.Id;

        public SkillBackedCellDefinition Definition => definition;
        public SkillDefinition SkillDefinition => skillDefinition;

        public override AffinityType Affinity
        {
            get => affinity;
            set { }
        }

        public void Initialize(SkillBackedCellDefinition skillCellDefinition)
        {
            definition = skillCellDefinition;
            skillDefinition = definition.SkillDefinition;
            affinity = definition.Affinity;
            Config = skillDefinition.Config;
        }

        public override string Description()
        {
            return skillDefinition.Description;
        }

        public override string Name()
        {
            return skillDefinition.DisplayName;
        }

        public override void Apply(Unit unit)
        {
            // Runtime skill creation will be wired in Todo3.
        }
    }
}
