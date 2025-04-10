using System;
using Units;
namespace Model.Tetri.Skills
{
    [Serializable]
    public class Rush : Cell, IBaseAttribute
    {
        private Units.Skills.Rush skillInstance = new (); // 实例化一个Burn对象
        public void ApplyAttributes(Unit unit)
        {
            unit.AddSkill(skillInstance);
        }

        public override string Description()
        {
            return $"向前冲刺 {skillInstance.rushDuration} 秒, 对碰撞到的敌人造成 " +
                $"基础攻击力 + (冲刺速度 × {skillInstance.damageMultipierBySpeed}) 的伤害. " +
                $"技能冷却时间: {skillInstance.cooldown} 秒.";
        }

        public override string Name()
        {
            return skillInstance.skillName;
        }
    }
}