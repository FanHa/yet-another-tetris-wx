using System;
using Units;

namespace Model.Tetri.Skills
{
    [Serializable]
    public abstract class Skill : Cell
    {
        protected abstract Units.Skills.Skill SkillInstance { get; }

        public override void Apply(Unit unit)
        {
            unit.AddSkill(SkillInstance); // 为单位添加技能
        }

        public override string Description()
        {
            return SkillInstance.Description(); // 返回技能描述
        }

        public override string Name()
        {
            return SkillInstance.Name(); // 返回技能名称
        }
    }
}
