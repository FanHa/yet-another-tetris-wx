using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.Collections;

namespace Units.Projectiles
{
    public class Bomb : Projectile
    {
        protected Units.Unit.Faction targetFaction; // 所属阵营
        protected float explosionRadius; // 爆炸范围

        public void Init(Units.Unit caster, Transform target, Damages.Damage damage, Units.Unit.Faction targetFaction, float explosionRadius)
        {

            base.Init(caster, target, damage);
            this.targetFaction = targetFaction;
            this.explosionRadius = explosionRadius;
        }

        protected override void HandleHitTarget()
        {
            Moving =false;
            StartCoroutine(ExplodeAfterDelay(2f));

        }

        protected virtual IEnumerator ExplodeAfterDelay(float delay)
        {
            // 停留在目标位置
            yield return new WaitForSeconds(delay);

            // 在爆炸范围内查找敌人
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
            foreach (var collider in hitColliders)
            {
                var targetUnit = collider.GetComponent<Unit>();
                if (targetUnit != null && targetUnit.faction == targetFaction)
                {
                    var explosionDamage = new Units.Damages.Damage(damage.Value, damage.Type)
                        .SetSourceLabel(damage.SourceLabel)
                        .SetSourceUnit(damage.SourceUnit)
                        .SetTargetUnit(targetUnit);

                    // 对敌人造成伤害
                    targetUnit.TakeDamage(explosionDamage);
                }
            }

            // 显示爆炸效果（可选）
            Debug.Log("Explosion at " + transform.position);

            // 销毁投射物
            Destroy(gameObject);
            Destroy(target.gameObject);
        }

        private void OnDrawGizmosSelected()
        {
            // 绘制爆炸范围的Gizmos
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, explosionRadius);
        }
    }
}