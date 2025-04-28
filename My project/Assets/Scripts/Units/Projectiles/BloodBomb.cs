using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Units.Projectiles
{
    public class BloodBomb : Bomb
    {
        private float healthReturnPercentage; // 每命中一个敌人返还的血量百分比

        public void Init(Units.Unit caster, Transform target, float speed, Damages.Damage damage, Units.Unit.Faction targetFaction, float explosionRadius, float healthReturnPercentage)
        {
            
            base.Init(caster, target, speed, damage, targetFaction, explosionRadius);
            this.healthReturnPercentage = healthReturnPercentage;
        }

        protected override IEnumerator ExplodeAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);

            // 在爆炸范围内查找敌人
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
            foreach (var collider in hitColliders)
            {
                var unit = collider.GetComponent<Unit>();
                if (unit != null && unit.faction != caster.faction)
                {
                    // 对敌人造成伤害
                    Damages.Damage explosionDamage = new(
                        damage.Value,
                        damage.SourceName,
                        damage.Type,
                        damage.SourceUnit,
                        unit,
                        damage.Buffs
                    );
                    unit.TakeDamage(explosionDamage);

                    Damages.Damage healthToReturn = new(
                        
                        -damage.Value * (healthReturnPercentage / 100f),
                        damage.SourceName + "回复",
                        Damages.DamageType.Skill,
                        damage.SourceUnit,
                        caster,
                        new List<Buffs.Buff>()
                    );
                    caster.TakeDamage(healthToReturn);
                }
            }

            // 显示爆炸效果（可选）
            Debug.Log($"BloodBomb exploded at {transform.position}, dealing {damage.Value} damage.");

            // 销毁投射物
            Destroy(gameObject);
            Destroy(target.gameObject);
        }
    }
}