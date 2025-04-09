using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Units.Skills
{
    public class Weak : Skill
    {
        public string skillName = "虚弱"; // 技能名称
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

        public override void Execute(Unit caster)
        {
            // 找到射程范围内的所有敌人
            Collider2D[] colliders = Physics2D.OverlapCircleAll(caster.transform.position, caster.attackRange);
            var enemiesInRange = colliders
                .Select(collider => collider.GetComponent<Unit>())
                .Where(enemy => enemy != null && enemy.faction != caster.faction) // 确保是敌人
                .ToList();

            if (enemiesInRange.Count == 0)
            {
                Debug.LogWarning("No valid targets found within range for Weak.");
                return;
            }

            // 随机选择一个敌人
            Unit targetEnemy = enemiesInRange[Random.Range(0, enemiesInRange.Count)];

            // 创建虚弱Buff实例
            Buffs.Buff weakDebuff = new Units.Buffs.Weak();

            // 给目标敌人添加虚弱Debuff
            targetEnemy.AddBuff(weakDebuff);

            Debug.Log($"Applied Weak debuff to {targetEnemy.name} for {weakDebuff.Duration()} seconds.");
            lastUsedTime = Time.time; // 更新上次使用时间
        }
    }
}