using System;
using Units;

namespace Model.Tetri.Skills
{
    [Serializable]
    public class Rush : SkillBase
    {
        protected override Units.Skills.Skill SkillInstance { get; } = new Units.Skills.Rush();
    }
}