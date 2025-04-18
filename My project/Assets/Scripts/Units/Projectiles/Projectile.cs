using UnityEngine;
using System.Collections.Generic;
using System;

namespace Units.Projectiles
{
    public class Projectile : MonoBehaviour
    {
        public Transform target; // 目标物体
        public float speed; // 移动速度
        public Units.Damages.Damage damage; // 伤害值

        void Update()
        {
            // 如果目标不存在，销毁自身
            if (target == null)
            {
                Destroy(gameObject);
                return;
            }


            Vector3 direction = (target.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;

            // 设置投射物的正前方（transform.up）朝向目标方向
            transform.up = direction;

            // 检测是否触碰到目标
            if (Vector3.Distance(transform.position, target.position) < 0.2f)
            {
                OnHitTarget();
            }
            
        }

        protected virtual void OnHitTarget()
        {
            // 对目标造成伤害
            Unit targetUnit = damage.TargetUnit;
            if (targetUnit != null)
            {
                targetUnit.TakeDamage(damage);
                damage.SourceUnit.TriggerOnAttackHit( damage);


            }
            // 销毁投射物
            Destroy(gameObject);
        }
    }
}