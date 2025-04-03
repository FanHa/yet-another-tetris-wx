using System;
using Units;
namespace Model.Tetri
{
    [Serializable]
    public class Rush : Cell, IBaseAttribute
    {
        public Units.Skills.Rush skillInstance = new (); // 实例化一个Burn对象
        public void ApplyAttributes(Unit unit)
        {
            unit.skills.Add(skillInstance);
        }

        public override string Description()
        {
            return "技能: 冲锋, 向前冲刺1秒, 对碰撞到的敌人造成 基础伤害10 + 质量*8 的伤害" +
                   "技能冷却时间: " + skillInstance.Cooldown() + "秒,";
        }
    }
}