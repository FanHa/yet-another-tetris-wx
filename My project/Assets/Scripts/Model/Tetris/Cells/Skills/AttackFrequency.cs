using System;
using Units;

namespace Model.Tetri.Skills
{
    [Serializable]
    public class AttackFrequency : Cell, IBaseAttribute
    {
        private Units.Skills.AttackFrequency skillInstance = new(); // 实例化一个AttackFrequency对象

        public void ApplyAttributes(Unit unit)
        {
            unit.AddSkill(skillInstance); // 为单位添加AttackFrequency技能
            
        }

        public override string Description()
        {
            return skillInstance.Description();
        }

        public override string Name()
        {
            return skillInstance.skillName; // 返回技能名称
        }
    }
}