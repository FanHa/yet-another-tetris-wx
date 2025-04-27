using System;
using Units;

namespace Model.Tetri.Skills
{
    [Serializable]
    public class ProjectileHail : Skill
    {
        protected override Units.Skills.Skill SkillInstance { get; } = new Units.Skills.ProjectileHail();
    }
}