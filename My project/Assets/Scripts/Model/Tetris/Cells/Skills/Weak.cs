using System;
using Units;

namespace Model.Tetri.Skills
{
    [Serializable]
    public class Weak : SkillBase
    {
        protected override Units.Skills.Skill SkillInstance { get; } = new Units.Skills.Weak();
    }
}