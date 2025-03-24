using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UI;
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
        private HealthBar healthBar;

        [SerializeField] private GameObject damageTextPrefab; // 伤害显示的Prefab
        [SerializeField] private SpriteRenderer bodySpriteRenderer;
        [SerializeField] private Canvas unitCanvas; // Unit的Canvas

        public bool isRanged; // 是否为远程单位
        public GameObject projectilePrefab; // 投射物预制体
        public Transform projectileSpawnPoint; // 投射物生成位置

        private void Awake()
        {
            // 获取当前对象的 Animator 组件
            animator = GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogWarning("Animator component not found on the same GameObject.");
            }
            healthBar = GetComponentInChildren<HealthBar>();
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
                    Unit enemyUnit = targetEnemy.GetComponent<Unit>();
                    Attack(enemyUnit);
                    lastAttackTime = Time.time;
                }
            }
        }

        public void Attack(Unit target)
        {
            animator.SetTrigger("Attack");
            if (isRanged)
            {
                // 发射投射物
                FireProjectile(target);
            }
            else
            {
                // 近战攻击逻辑
                target.TakeDamage(attackDamage);
            }
        }

        private void FireProjectile(Unit target)
        {
            if (projectilePrefab != null && projectileSpawnPoint != null)
            {
                GameObject projectileObject = Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);
                Projectile projectile = projectileObject.GetComponent<Projectile>();
                if (projectile != null)
                {
                    projectile.target = target.transform;
                    projectile.damage = attackDamage;
                }
            }
        }

        public void TakeDamage(float damage)
        {
            currentHP -= damage;
            ShowDamageText(damage);
            healthBar.UpdateHealthBar(currentHP, maxHP);
            CheckHealth();
        }

        private void ShowDamageText(float damage)
        {
            if (damageTextPrefab != null && unitCanvas != null)
            {
                GameObject damageTextInstance = Instantiate(damageTextPrefab, unitCanvas.transform); // 指定Canvas为父对象
                damageTextInstance.transform.localPosition = Vector3.zero; // 可根据需要调整位置
                damageTextInstance.transform.rotation = Quaternion.identity; // 确保文本不随物体旋转
                TextMeshProUGUI damageText = damageTextInstance.GetComponent<TextMeshProUGUI>();
                if (damageText != null)
                {
                    damageText.text = damage.ToString();
                    StartCoroutine(FadeAndDestroyDamageText(damageTextInstance, damageText));
                }
            }
        }

        private IEnumerator FadeAndDestroyDamageText(GameObject damageTextInstance, TextMeshProUGUI damageText)
        {
            Color originalColor = damageText.color;
            float duration = 1f; // 显示时间为1秒
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float alpha = Mathf.Lerp(1f, 0f, elapsedTime / duration); // 渐变透明度
                damageText.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
                yield return null;
            }

            Destroy(damageTextInstance); // 销毁实例
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

        /// <summary>
        /// Sets the faction of the unit and updates the color of the body sprite renderer accordingly.
        /// </summary>
        /// <param name="faction">The faction to set for the unit.</param>
        public void SetFaction(Faction faction)
        {
            this.unitFaction = faction;
            bodySpriteRenderer.color = faction == Faction.FactionA ? Color.blue : Color.red;
        }

        
    }
}
