using System;
using Units;

namespace Model.Tetri.Skills
{
    [Serializable]
    public class BloodBomb : Skill
    {
        protected override Units.Skills.Skill SkillInstance { get; } = new Units.Skills.BloodBomb();
    }
}