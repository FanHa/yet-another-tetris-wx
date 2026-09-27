using System;
using Units;
using Units.Skills;

namespace Model.Tetri
{
    [Serializable]
    public class SkillCell : Cell
    {
        public override void Apply(Unit unit)
        {
            if (Definition is not SkillBackedCellDefinition skillDefinition || skillDefinition.SkillDefinition == null)
            {
                return;
            }

            Skill skill = SkillFactory.Create(skillDefinition.SkillDefinition, Level);
            if (skill != null)
            {
                unit.AddSkill(skill);
            }
        }
    }
}
