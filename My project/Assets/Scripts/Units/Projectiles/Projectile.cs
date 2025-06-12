using UnityEngine;
using System.Collections.Generic;
using System;

namespace Units.Projectiles
{
    public class Projectile : MonoBehaviour
    {
        protected Units.Unit caster;
        protected Transform target; // 目标物体, 很多投射物都是投射到目标单位身边的一个虚拟爆炸点,所以target不使用Unit
        [SerializeField] protected float speed; // 移动速度
        protected Damages.Damage damage;
        protected bool Initialized = false;
        protected bool Moving = true;
        private Vector3 offsetDirection; // 偏移方向
        private bool isOffsetPhase = true; // 是否处于偏移阶段
        private float offsetDuration = 0.2f; // 偏移持续时间
        private float offsetTimer = 0f; // 偏移计时器
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
                if (isOffsetPhase)
                {
                    // 偏移阶段
                    transform.position += 0.3f * speed * Time.deltaTime * offsetDirection;
                    offsetTimer += Time.deltaTime;

                    if (offsetTimer >= offsetDuration)
                    {
                        isOffsetPhase = false; // 偏移阶段结束
                    }
                }
                else {
                    // 朝目标移动
                    Vector3 direction = (target.position - transform.position).normalized;
                    transform.position += speed * Time.deltaTime * direction;
                    // 设置投射物的正前方（transform.up）朝向目标方向
                    transform.up = direction;
                }
                
                
            }
        }

        public void Init(Units.Unit caster, Transform target, Damages.Damage damage)
        {
            this.caster = caster;
            this.target = target;
            this.damage = damage;

            // 计算偏移方向（随机向左或向右）
            Vector3 toTarget = (target.position - transform.position).normalized;
            offsetDirection = Quaternion.Euler(0, 0, UnityEngine.Random.value > 0.5f ? 90 : -90) * toTarget;

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
                caster.TriggerAttackHit(targetUnit, damage);
                targetUnit.TakeHit(caster, ref damage);
                // targetUnit.TakeDamage(damage);
                
            }
            Destroy(gameObject);
        }
    }
}