using UnityEngine;
using System.Collections.Generic;

namespace Units.Projectiles
{
    public class Bomb : MonoBehaviour
    {
        public Units.Unit.Faction faction; // 所属阵营
        public Transform target; // 目标物体
        public float speed = 4f; // 移动速度
        public float damage = 50; // 伤害值
        public float explosionRadius = 2f; // 爆炸范围

        protected void Update()
        {
            // 如果目标位置不存在，销毁自身
            if (target == null)
            {
                Destroy(gameObject);
                return;
            }

            // 朝目标位置移动
            Vector3 direction = (target.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;

            // 检测是否到达目标位置
            if (Vector3.Distance(transform.position, target.position) < 0.2f)
            {
                Explode();
            }
        }

        private void Explode()
        {
            // 在爆炸范围内查找敌人
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
            foreach (var collider in hitColliders)
            {
                var unit = collider.GetComponent<Unit>();
                if (unit != null && unit.faction != faction)
                {
                    // 对敌人造成伤害
                    unit.TakeDamage(damage, new List<Buff>());

                }
            }
            // 显示爆炸效果（可选）
            Debug.Log("Explosion at " + transform.position);

            // 销毁投射物
            Destroy(gameObject);
        }

        private void OnDrawGizmosSelected()
        {
            // 绘制爆炸范围的Gizmos
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, explosionRadius);
        }
    }
}