using System;
using Units;

namespace Model.Tetri.Skills
{
    [Serializable]
    public class AttackFrequency : SkillBase
    {
        protected override Units.Skills.Skill SkillInstance { get; } = new Units.Skills.AttackFrequency();
    }
}