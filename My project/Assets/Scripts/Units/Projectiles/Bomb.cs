using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

namespace Units.Projectiles
{
    public class Bomb : Projectile
    {
        public Units.Unit.Faction faction; // 所属阵营
        public float explosionRadius = 2f; // 爆炸范围


        protected override void OnHitTarget()
        {
            // 在爆炸范围内查找敌人
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
            foreach (var collider in hitColliders)
            {
                var unit = collider.GetComponent<Unit>();
                if (unit != null && unit.faction != faction)
                {
                    // 对敌人造成伤害
                    unit.TakeDamage(damage);
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