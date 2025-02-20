using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Units
{
    public class Unit : MonoBehaviour
    {
        public enum Faction
        {
            FactionA,
            FactionB
        }

        public event Action<Unit> OnDeath;

        public Faction unitFaction; // 单位的阵营
        public float moveSpeed = 5f; // 移动速度
        public float attackRange = 0.5f; // 攻击范围
        public float attackCooldown = 1f; // 攻击冷却时间
        public float maxHP = 100f; // 最大生命值
        public float attackDamage = 10f; // 攻击力
        public float minDistance = 0.5f; // 与敌人保持的最小距离

        private float detectionRadius = 30f;
        protected Transform targetEnemy;
        protected float lastAttackTime;
        private float currentHP;
        private Rigidbody2D rb;
        private Animator animator;

        private void Awake()
        {
            // 获取当前对象的 Animator 组件
            animator = GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogWarning("Animator component not found on the same GameObject.");
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            lastAttackTime = -attackCooldown; // 确保一开始就可以攻击
            currentHP = maxHP; // 初始化当前生命值
            rb = GetComponent<Rigidbody2D>(); // 获取 Rigidbody2D 组件

        }

        // Update is called once per frame
        protected void Update()
        {
            FindClosestEnemy();
            MoveTowardsEnemy();
            AttackEnemy();
        }

        protected void FindClosestEnemy()
        {
            // todo 缩小范围,只检测某个object下的enemy
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, detectionRadius);
            float closestDistance = Mathf.Infinity;
            Transform closestEnemy = null;

            foreach (Collider2D collider in colliders)
            {
                Unit unit = collider.GetComponent<Unit>();
                if (unit != null && unit.unitFaction != unitFaction) // 判断是否为敌对阵营
                {
                    float distance = Vector2.Distance(transform.position, collider.transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestEnemy = collider.transform;
                    }
                }
            }

            targetEnemy = closestEnemy;
        }

        protected virtual void MoveTowardsEnemy()
        {
            if (targetEnemy != null)
            {
                float distance = Vector2.Distance(transform.position, targetEnemy.position);
                if (distance > attackRange && distance > minDistance)
                {
                    // todo 调整自己的方向
                    Vector2 direction = (targetEnemy.position - transform.position).normalized;
                    Vector2 newPosition = Vector2.MoveTowards(rb.position, targetEnemy.position, moveSpeed * Time.deltaTime);
                    rb.MovePosition(newPosition); // 使用 Rigidbody2D 的 MovePosition 方法
                }
            }
        }

        protected void AttackEnemy()
        {
            if (targetEnemy != null)
            {
                float distance = Vector2.Distance(transform.position, targetEnemy.position);
                if (distance <= attackRange && Time.time >= lastAttackTime + attackCooldown)
                {
                    // 发起攻击
                    animator.SetTrigger("Attack");
                    Unit enemyUnit = targetEnemy.GetComponent<Unit>();
                    enemyUnit?.TakeDamage(attackDamage);
                    lastAttackTime = Time.time;
                }
            }
        }

        public void TakeDamage(float damage)
        {
            currentHP -= damage;
            CheckHealth();
        }

        private void CheckHealth()
        {
            if (currentHP <= 0)
            {
                OnDeath?.Invoke(this);
                Destroy(gameObject);
            }
        }

        void OnDrawGizmosSelected()
        {
            // 绘制检测范围的Gizmos
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, detectionRadius);

            // 绘制攻击范围的Gizmos
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }
    }
}
