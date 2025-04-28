using UnityEngine;
using System.Collections.Generic;
using System;

namespace Units.Projectiles
{
    public class Projectile : MonoBehaviour
    {
        protected Units.Unit caster;
        protected Transform target; // 目标物体, 很多投射物都是投射到目标单位身边的一个虚拟爆炸点,所以target不使用Unit
        protected float speed; // 移动速度
        protected Damages.Damage damage;
        protected bool Initialized = false;
        protected bool Moving = true;

        void Update()
        {
            if (Initialized == false)
            {
                return;
            }
            // 如果目标不存在，销毁自身
            if (target == null)
            {
                Destroy(gameObject);
                return;
            }
            
            if (Moving)
            {
                Vector3 direction = (target.position - transform.position).normalized;
                transform.position += speed * Time.deltaTime * direction;

                // 设置投射物的正前方（transform.up）朝向目标方向
                transform.up = direction;
            }
        }

        public void Init(Units.Unit caster, Transform target, float speed, Damages.Damage damage)
        {
            
            this.caster = caster;
            this.target = target;
            this.speed = speed;
            this.damage = damage;

            Initialized = true;
        }
        
        protected virtual void OnTriggerEnter2D(Collider2D other)
        {
            if (other.transform == target)
            {
                HandleHitTarget();
            }
        }

        protected virtual void HandleHitTarget()
        {
            if (target.TryGetComponent<Unit>(out var targetUnit))
            {
                targetUnit.TakeDamage(damage);
                caster.TriggerOnAttackHit( damage);
            }
            Destroy(gameObject);
        }
    }
}