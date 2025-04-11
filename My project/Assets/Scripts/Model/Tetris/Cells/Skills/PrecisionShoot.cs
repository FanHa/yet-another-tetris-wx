using System;
using Units;

namespace Model.Tetri.Skills
{
    [Serializable]
    public class PrecisionShoot : Skill
    {
        protected override Units.Skills.Skill SkillInstance { get; } = new Units.Skills.PrecisionShoot();
    }
}