using System;
using Units;

namespace Model.Tetri
{
    [Serializable]
    public class FreezeShield : Cell, IBaseAttribute
    {
        private Units.Skills.FreezeShield skillInstance = new(); // 实例化一个 FreezeShield 技能对象

        public void ApplyAttributes(Unit unit)
        {
            unit.AddSkill(skillInstance); // 为单位添加 FreezeShield 技能
        }

        public override string Description()
        {
            return skillInstance.Description(); // 返回技能描述
        }

        public override string Name()
        {
            return skillInstance.skillName; // 返回技能名称
        }
    }
}