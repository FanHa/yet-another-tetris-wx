using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        public float minDistance = 0.1f; // 与敌人保持的最小距离
        public float attackTargetNumber = 1; // 攻击目标数量
        
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
        private List<Transform> targetEnemies;
        private Transform factionAParent;
        private Transform factionBParent;

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
            FindClosestEnemies();
            AttackEnemies();
        }

        protected void FixedUpdate()
        {
            MoveTowardsEnemy();
        }

        public void SetFactionParent(Transform factionA, Transform factionB)
        {
            factionAParent = factionA;
            factionBParent = factionB;
        }

        protected void FindClosestEnemies()
        {
            Transform enemyParent = unitFaction == Faction.FactionA ? factionBParent : factionAParent;
            if (enemyParent == null)
            {
                Debug.LogWarning("Enemy parent is not set for this unit.");
                return;
            }
            List<Transform> enemiesInRange = new List<Transform>();
            
            foreach (Transform enemy in enemyParent)
            {
                Unit unit = enemy.GetComponent<Unit>();
                if (unit != null && unit.unitFaction != unitFaction) // 判断是否为敌对阵营
                {
                    enemiesInRange.Add(enemy);
                }
            }

            // 按距离排序并选择最近的 attackTargetNumber 个敌人
            targetEnemies = enemiesInRange
                .OrderBy(enemy => Vector2.Distance(transform.position, enemy.position))
                .Take((int)attackTargetNumber)
                .ToList();
        }

        protected virtual void MoveTowardsEnemy()
        {
            if (targetEnemies != null && targetEnemies.Count > 0)
            {
                Transform closestEnemy = targetEnemies.FirstOrDefault();
                if (closestEnemy != null)
                {
                    float distance = Vector2.Distance(transform.position, closestEnemy.position);

                    // 如果距离大于攻击范围和最小距离，则移动
                    if (distance > attackRange && distance > minDistance)
                    {
                        // 调整自己的方向
                        Vector2 direction = (closestEnemy.position - transform.position).normalized;
                        Vector2 newPosition = Vector2.MoveTowards(rb.position, closestEnemy.position, moveSpeed * Time.deltaTime);
                        rb.MovePosition(newPosition); // 使用 Rigidbody2D 的 MovePosition 方法
                    }
                }
            }
        }

        protected void AttackEnemies()
        {
            if (targetEnemies != null && targetEnemies.Count > 0)
            {
                bool attacked = false;
                foreach (var target in targetEnemies)
                {
                    float distance = Vector2.Distance(transform.position, target.position);
                    if (distance <= attackRange && Time.time >= lastAttackTime + attackCooldown)
                    {
                        Unit enemyUnit = target.GetComponent<Unit>();
                        if (enemyUnit != null)
                        {
                            Attack(enemyUnit);
                            attacked = true;
                        }
                    }
                }
                if (attacked)
                {
                    lastAttackTime = Time.time;
                }
            }

        }

        public virtual void Attack(Unit target)
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
                target.TakeDamage(attackDamage); // Use attackDamage instead of attackDamage
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
                    projectile.damage = attackDamage; // Use attackDamage instead of attackDamage
                }
            }
        }


        public virtual void TakeDamage(float damage)
        {
            currentHP -= damage;
            ShowDamageText(damage);
            healthBar.UpdateHealthBar(currentHP, maxHP); // Use GetMaxHP() instead of maxHP
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
