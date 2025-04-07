using System;
using Units;
namespace Model.Tetri
{
    [Serializable]
    public class Repel : Cell, IBaseAttribute
    {
        private Units.Skills.Repel skillInstance = new (); // 实例化一个Burn对象
        public void ApplyAttributes(Unit unit)
        {
            unit.AddSkill(skillInstance);
        }

        public override string Description()
        {
            return $"击退目标," + $"技能冷却时间: {skillInstance.cooldown} 秒";
        }

        public override string Name()
        {
            return skillInstance.skillName;
        }
    }
}