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
        public List<int> moveSpeedPercentageModifiers = new List<int>(); // 移动速度百分比修正列表
        public float attackRange = 0.5f; // 攻击范围
        public float attackCooldown = 1f; // 攻击冷却时间
        public float maxHP = 100f; // 最大生命值
        public List<int> maxHPPercentageModifiers = new List<int>(); // 百分比修正列表

        public float attackPower = 10f; // 攻击力
        public List<int> attackPowerPercentageModifiers = new List<int>(); // 百分比修正列表

        public List<int> massPercentageModifiers = new List<int>(); // 百分比修正列表

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

        public List<Buff> attackEffects = new List<Buff>(); // 攻击效果列表

        [SerializeField] private Dictionary<string, Buff> activeBuffs = new Dictionary<string, Buff>();
        private float buffTimer = 0f; // 用于控制每秒触发一次 Debuff

        private void Awake()
        {
            // 获取当前对象的 Animator 组件
            animator = GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogWarning("Animator component not found on the same GameObject.");
            }
            healthBar = GetComponentInChildren<HealthBar>();
            rb = GetComponent<Rigidbody2D>(); // 获取 Rigidbody2D 组件
        }

        // Start is called before the first frame update
        void Start()
        {
            lastAttackTime = -attackCooldown; // 确保一开始就可以攻击
            int totalMaxHPPercentage = 100; // 初始化总百分比
            foreach (int modifier in maxHPPercentageModifiers) {
                totalMaxHPPercentage += modifier; // 计算总百分比修正值
            }
            maxHP *= totalMaxHPPercentage / 100f; // 应用百分比修正
            currentHP = maxHP; // 初始化当前生命值
            
            int totalMassPercentage = 100; // 初始化总百分比
            foreach (int modifier in massPercentageModifiers)
            {
                totalMassPercentage += modifier; // 计算总百分比修正值
            }
            rb.mass *= totalMassPercentage / 100f; // 应用百分比修正

            int totalMoveSpeedPercentage = 100; // 初始化总百分比
            foreach (int modifier in moveSpeedPercentageModifiers)
            {
                totalMoveSpeedPercentage += modifier; // 计算总百分比修正值
            }
            moveSpeed *= totalMoveSpeedPercentage / 100f; // 应用百分比修正
        }

        // Update is called once per frame
        void Update()
        {
            FindClosestEnemies();
            AttackEnemies();
            // todo 协程或invokeRepeating
            BuffEffect(Time.deltaTime);
        }

        void FixedUpdate()
        {
            MoveTowardsEnemy();
        }

        public void AddBuff(Units.Buff buff)
        {
            if (activeBuffs.TryGetValue(buff.Name(), out var existingBuff))
            {
                // 如果状态已存在，则刷新持续时间
                existingBuff.RefreshDuration();
            }
            else
            {
                // 添加新的状态并立即应用
                activeBuffs[buff.Name()] = buff;
                buff.Apply(this);
            }
        }

        private void BuffEffect(float deltaTime)
        {
            buffTimer += deltaTime;
            if (buffTimer >= 1f)
            {

                var expiredBuffs = new List<string>();

                foreach (var kvp in activeBuffs)
                {
                    var buff = kvp.Value;
                    if (buff.IsExpired())
                    {
                        buff.Remove(this);
                        expiredBuffs.Add(kvp.Key);
                    } else {
                        buff.Affect(this);
                    } 
                }

                // 移除已过期的状态
                foreach (var buffName in expiredBuffs)
                {
                    activeBuffs.Remove(buffName);
                }
                buffTimer = 0f;

            }
        }



        private float GetFinalAttackPower()
        {
            int totalPercentage = 100;
            foreach (var modifier in attackPowerPercentageModifiers)
            {
                totalPercentage += modifier;
            }

            // 应用百分比修正
            return attackPower * (totalPercentage / 100f)/ attackTargetNumber;
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
            if (Time.time < lastAttackTime + attackCooldown) 
                return; // 检查攻击冷却时间
            if (targetEnemies != null && targetEnemies.Count > 0)
            {
                bool attacked = false;
                foreach (var target in targetEnemies)
                {
                    float distance = Vector2.Distance(transform.position, target.position);
                    if (distance <= attackRange)
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
            float finalAttackPower = GetFinalAttackPower(); // 获取修正后的攻击力
            if (isRanged)
            {
                // 发射投射物
                FireProjectile(target, finalAttackPower);
            }
            else
            {
                // 近战攻击逻辑
                target.TakeDamage(finalAttackPower);
                // 触发所有攻击效果
                foreach (var effect in attackEffects)
                {
                    target.AddBuff(effect); // 添加效果到目标单位
                }
            }
        }

        private void FireProjectile(Unit target, float damage)
        {
            if (projectilePrefab != null && projectileSpawnPoint != null)
            {
                GameObject projectileObject = Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);
                Projectile projectile = projectileObject.GetComponent<Projectile>();
                if (projectile != null)
                {
                    projectile.target = target.transform;
                    projectile.damage = damage; // 使用修正后的攻击力
                    projectile.debuffs = new List<Buff>(attackEffects); // 传递攻击效果作为Debuff
                }
            }
        }



        public virtual void TakeDamage(float damageReceived)
        {
            currentHP -= damageReceived;
            ShowDamageText(damageReceived);
            healthBar.UpdateHealthBar(currentHP, maxHP); // Use GetMaxHP() instead of maxHP
            CheckHealth();
        }

        private void ShowDamageText(float damageReceived)
        {
            if (damageTextPrefab != null && unitCanvas != null)
            {
                GameObject damageTextInstance = Instantiate(damageTextPrefab, unitCanvas.transform); // 指定Canvas为父对象
                damageTextInstance.transform.localPosition = Vector3.zero; // 可根据需要调整位置
                damageTextInstance.transform.rotation = Quaternion.identity; // 确保文本不随物体旋转
                TextMeshProUGUI damageText = damageTextInstance.GetComponent<TextMeshProUGUI>();
                if (damageText != null)
                {
                    damageText.text = damageReceived.ToString();
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
        /// <param name="faction">The faction to set for the unit.</param>
        public void SetFaction(Faction faction)
        {
            this.unitFaction = faction;
            bodySpriteRenderer.color = faction == Faction.FactionA ? Color.blue : Color.red;
        }

        
    }
}
