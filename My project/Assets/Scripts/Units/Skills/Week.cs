using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Units.Skills
{
    public class Week : Skill
    {
        public string skillName = "虚弱"; // 技能名称
        public override float cooldown => 10f; // 技能冷却时间

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
                Debug.LogWarning("No valid targets found within range for Week.");
                return;
            }

            // 随机选择一个敌人
            Unit targetEnemy = enemiesInRange[Random.Range(0, enemiesInRange.Count)];

            // 创建虚弱Buff实例
            Buff weakDebuff = new Units.Week();

            // 给目标敌人添加虚弱Debuff
            targetEnemy.AddBuff(weakDebuff);

            Debug.Log($"Applied Week debuff to {targetEnemy.name} for {weakDebuff.Duration()} seconds.");
            lastUsedTime = Time.time; // 更新上次使用时间
        }
    }
}