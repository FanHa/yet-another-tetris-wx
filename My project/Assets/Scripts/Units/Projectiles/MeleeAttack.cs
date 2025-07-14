using UnityEngine;

namespace Units.Projectiles
{
    public class MeleeAttack : Projectile
    {
        protected override void Update()
        {
            if (!Initialized)
                return;

            // 近战攻击：直接瞬移到目标位置并触发命中
            if (target == null)
            {
                Destroy(gameObject);
                return;
            }

            // 直接设置位置到目标（可选：或做极快移动）
            transform.position = target.position;

            // 触发命中并销毁
            HandleHitTarget();
        }

        // 近战攻击不需要偏移和移动表现，重写 Init 以跳过偏移逻辑
        public override void Init(Units.Unit caster, Transform target, Damages.Damage damage)
        {
            this.caster = caster;
            this.target = target;
            this.damage = damage;
        }
    }
}