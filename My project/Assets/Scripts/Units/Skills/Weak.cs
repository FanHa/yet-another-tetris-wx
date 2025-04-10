using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Units.Skills
{
    public class Weak : Skill
    {
        public override float cooldown => 10f; // 技能冷却时间

        private Units.Buffs.Weak buffTemplate = new(); // 虚弱Buff模板

        public override string Description()
        {
            // 从 buffTemplate 获取虚弱效果的属性
            float attackReduction = buffTemplate.attackReductionPercentage;
            float damageIncrease = buffTemplate.damageTakenIncreasePercentage;
            float duration = buffTemplate.Duration();

            // 返回技能描述
            return $"随机对射程范围内的一个敌人施加虚弱效果," +
                $"减少其攻击力 {attackReduction}% 并增加其受到的伤害 {damageIncrease}%," +
                $"持续 {duration} 秒," +
                $"技能冷却时间为 {cooldown} 秒,";
        }

        protected override void ExecuteCore(Unit caster)
        {
            var enemiesInRange = FindEnemiesInRange(caster, caster.attackRange);
            if (enemiesInRange.Count == 0)
            {
                Debug.LogWarning("No valid targets found within range for Weak.");
                return;
            }

            Unit targetEnemy = enemiesInRange[Random.Range(0, enemiesInRange.Count)];
            Buffs.Buff weakDebuff = new Units.Buffs.Weak();
            targetEnemy.AddBuff(weakDebuff);

            Debug.Log($"Applied Weak debuff to {targetEnemy.name} for {weakDebuff.Duration()} seconds.");
        }

        public override string Name()
        {
            return "虚弱"; // 返回技能名称
        }
    }
}