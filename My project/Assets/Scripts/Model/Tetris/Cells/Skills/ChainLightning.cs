using System;
using Units;

namespace Model.Tetri.Skills
{
    [Serializable]
    public class ChainLightning : Skill
    {
        protected override Units.Skills.Skill SkillInstance { get; } = new Units.Skills.ChainLightning();
    }
}