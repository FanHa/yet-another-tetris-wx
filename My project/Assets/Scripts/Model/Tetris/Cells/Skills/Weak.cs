using System;
using Units;

namespace Model.Tetri.Skills
{
    [Serializable]
    public class Weak : Cell, IBaseAttribute
    {
        private Units.Skills.Weak skillInstance = new(); // 实例化一个 Weak 技能对象
        private static Units.Buffs.Weak sharedBuff = new(); // 共享的 Buff 实例


        public void ApplyAttributes(Unit unit)
        {
            unit.AddSkill(skillInstance); // 为单位添加 Weak 技能
        }

        public override string Description()
        {
            // 从共享的 Buff 实例中获取属性
            float attackReduction = sharedBuff.attackReductionPercentage;
            float damageIncrease = sharedBuff.damageTakenIncreasePercentage;
            float duration = sharedBuff.Duration();

            // 返回技能描述
            return $"随机对射程范围内的一个敌人施加虚弱效果," +
                   $"减少其攻击力 {attackReduction}% 并增加其受到的伤害 {damageIncrease}%," +
                   $"持续 {duration} 秒," +
                   $"技能冷却时间: {skillInstance.cooldown} 秒,";
        }

        public override string Name()
        {
            return skillInstance.skillName; // 返回技能名称
        }
    }
}