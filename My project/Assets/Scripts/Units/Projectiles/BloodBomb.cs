using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Units.Projectiles
{
    public class BloodBomb : Bomb
    {
        private float healthReturnPercentage; // 每命中一个敌人返还的血量百分比

        public void Init(Units.Unit caster, Transform target, Damages.Damage damage, Units.Unit.Faction targetFaction, float explosionRadius, float healthReturnPercentage)
        {
            
            base.Init(caster, target, damage, targetFaction, explosionRadius);
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
                    var explosionDamage = new Damages.Damage(damage.Value, Damages.DamageType.Skill)
                        .SetSourceLabel(damage.SourceLabel)
                        .SetSourceUnit(damage.SourceUnit)
                        .SetTargetUnit(unit)
                        .SetBuffs(damage.Buffs);

                    unit.TakeDamage(explosionDamage);

                    var healthToReturn = new Damages.Damage(-damage.Value * (healthReturnPercentage / 100f), Damages.DamageType.Skill)
                        .SetSourceLabel(damage.SourceLabel + "回复")
                        .SetSourceUnit(damage.SourceUnit)
                        .SetTargetUnit(caster)
                        .SetBuffs(damage.Buffs);

                    caster.TakeDamage(healthToReturn);
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