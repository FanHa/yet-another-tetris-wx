using UnityEngine;

namespace Units
{
    public class Projectile : MonoBehaviour
    {
        // 目标物体
        public Transform target;

        // 移动速度
        public float speed = 5f;

        // 伤害值
        public float damage = 10;

        void Update()
        {
            // 如果目标不存在，销毁自身
            if (target == null)
            {
                Destroy(gameObject);
                return;
            }

            // 如果目标存在，朝目标移动
            if (target != null)
            {
                Vector3 direction = (target.position - transform.position).normalized;
                transform.position += direction * speed * Time.deltaTime;

                // 检测是否触碰到目标
                if (Vector3.Distance(transform.position, target.position) < 0.1f)
                {
                    OnHitTarget();
                }
            }
        }

        private void OnHitTarget()
        {
            // 对目标造成伤害
            var targetUnit = target.GetComponent<Unit>();
            if (targetUnit != null)
            {
                targetUnit.TakeDamage(damage);
            }

            // 销毁投射物
            Destroy(gameObject);
        }
    }
}