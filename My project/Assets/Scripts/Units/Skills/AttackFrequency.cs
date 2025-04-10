using System.Collections;
using System.Linq;
using UnityEngine;

namespace Units.Skills
{
    public class AttackFrequency : Skill
    {
        public string skillName = "攻速模块"; // 技能名称
        public override float cooldown => 10f; // 技能冷却时间
        private Units.Buffs.AttackFrequency buffTemplate = new();

        public override string Description()
        {

            // 返回技能描述
            return $"为射程范围内的一个随机友方单位（包括自己）添加攻速模块buff," +
                $"{buffTemplate.Description()}" +
                $"技能冷却时间为 {cooldown} 秒";
        }

        public override void Execute(Unit caster)
        {
            // 找到射程范围内的所有友方单位
            Collider2D[] colliders = Physics2D.OverlapCircleAll(caster.transform.position, caster.attackRange);
            var alliesInRange = colliders
                .Select(collider => collider.GetComponent<Unit>())
                .Where(ally => ally != null && ally.faction == caster.faction)
                .ToList();

            if (alliesInRange.Count == 0)
            {
                Debug.LogWarning("No valid allies found within range for FreezeShield.");
                return;
            }

            Unit targetAlly = alliesInRange[Random.Range(0, alliesInRange.Count)];
            targetAlly.AddBuff(new Units.Buffs.AttackFrequency());

            lastUsedTime = Time.time; // 更新上次使用时间
        }

    }
}