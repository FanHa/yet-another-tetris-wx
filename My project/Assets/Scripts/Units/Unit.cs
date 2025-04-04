using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Model.Tetri;
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
        public event Action<Transform, float> OnDamageTaken;

        public Faction faction; // 单位的阵营

        public Attribute moveSpeed = new Attribute(3f); // 移动速度
        
        public Attribute attacksPerTenSeconds = new Attribute(4f);
        protected float lastAttackTime = 0;

        public float attackRange = 0.5f; // 攻击范围

        public Attribute maxCore = new Attribute(100f); // 最大生命值
        public float currentCore;

        public Attribute attackPower = new Attribute(10f); // 攻击力

        public List<int> massPercentageModifiers = new List<int>(); // 百分比修正列表

        public float minDistance = 0.1f; // 与敌人保持的最小距离
        public float attackTargetNumber = 1; // 攻击目标数量
        
        private Animator animator;
        private HealthBar healthBar;

        // [SerializeField] private GameObject damageTextPrefab; // 伤害显示的Prefab
        [SerializeField] private SpriteRenderer bodySpriteRenderer;
        [SerializeField] private Canvas unitCanvas; // Unit的Canvas

        public bool isRanged; // 是否为远程单位
        public GameObject projectilePrefab; // 投射物预制体
        public GameObject bombPrefab; // TODO 暂时所有projectile的prefab都放到这里,以后再改
        public GameObject PrecisionArrowPrefab;
        public Transform projectileSpawnPoint; // 投射物生成位置
        public List<Transform> targetEnemies;
        private Transform factionAParent;
        private Transform factionBParent;

        public List<Buff> attackEffects = new List<Buff>(); // 攻击效果列表
        private List<Skills.Skill> _skills = new List<Skills.Skill>(); // 私有字段
        public IReadOnlyList<Skills.Skill> Skills => _skills; // 只读属性

        [SerializeField] private Dictionary<string, Buff> activeBuffs = new Dictionary<string, Buff>();
        private bool isActive = false; // 是否处于活动状态
        public bool moveable = true;
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
            int totalMassPercentage = 100; // 初始化总百分比
            foreach (int modifier in massPercentageModifiers)
            {
                totalMassPercentage += modifier; // 计算总百分比修正值
            }
            
        }

        // Update is called once per frame
        void Update()
        {
            if (isActive) {
                AttackEnemies();
            }
        }

        void FixedUpdate()
        {
            if (isActive)
            {
                MoveTowardsEnemy();
            }
        }

        public void Initialized()
        {
            currentCore = maxCore.finalValue; // 初始化当前核心值
            InvokeRepeating(nameof(BuffEffect), 1f, 1f);
            InvokeRepeating(nameof(FindClosestEnemies), 0f, 0.1f);

            foreach (Units.Skills.Skill skill in _skills)
            {
                skill.Init();
            }
            InvokeRepeating(nameof(CastSkills) , 0f, 0.2f); // 每秒调用一次技能
            isActive = true;
        }

        public void AddSkill(Skills.Skill newSkill)
        {
            // 检查是否已经存在相同类型的技能
            if (_skills.Any(skill => skill.GetType() == newSkill.GetType()))
            {
                return;
            }

            _skills.Add(newSkill);
        }

        public void StopAction()
        {
            CancelInvoke(nameof(BuffEffect));
            CancelInvoke(nameof(FindClosestEnemies));
            CancelInvoke(nameof(CastSkills));
            isActive = false;
        }


        private void AddBuff(Units.Buff buff)
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
                buff.RefreshDuration();
                buff.Apply(this);
            }
        }

        private void BuffEffect()
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
        }

        private void CastSkills()
        {
            foreach (var skill in _skills)
            {
                if (skill.IsReady())
                {
                    skill.Execute(this);
                }
            }
        }


        public void SetFactionParent(Transform factionA, Transform factionB)
        {
            factionAParent = factionA;
            factionBParent = factionB;
        }

        protected void FindClosestEnemies()
        {
            Transform enemyParent = faction == Faction.FactionA ? factionBParent : factionAParent;
            if (enemyParent == null)
            {
                Debug.LogWarning("Enemy parent is not set for this unit.");
                return;
            }
            List<Transform> enemiesInRange = new List<Transform>();
            
            foreach (Transform enemy in enemyParent)
            {
                Unit unit = enemy.GetComponent<Unit>();
                if (unit != null && unit.faction != faction) // 判断是否为敌对阵营
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
            if (!moveable) 
                return; // 如果不可移动，则直接返回
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
                        AdjustLookDirection(direction);
                        // 计算下一步位置
                        Vector2 newPosition = Vector2.MoveTowards(transform.position, closestEnemy.position, moveSpeed.finalValue * Time.deltaTime);

                        // 更新位置
                        transform.position = newPosition;
                    }
                }
            }
        }

        private void AdjustLookDirection(Vector2 direction)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; // 计算角度
            transform.rotation = Quaternion.Euler(0, 0, angle-90f);//设置自身旋转
        }

        protected void AttackEnemies()
        {
            float attackCooldown = 10f / attacksPerTenSeconds.finalValue;
            if (Time.time < lastAttackTime + attackCooldown) 
                return; // 检查攻击冷却时间
            if (targetEnemies != null && targetEnemies.Count > 0)
            {
                Transform closestEnemy = targetEnemies[0]; // 获取最近的敌人
                if (closestEnemy != null)
                {
                    float distance = Vector2.Distance(transform.position, closestEnemy.position);
                    if (distance <= attackRange) // 检查最近的敌人是否在攻击范围内
                    {
                        TriggerAttack();
                        lastAttackTime = Time.time; // 更新攻击时间
                    }
                }
            }
        }

        private void TriggerAttack()
        {
            animator.SetTrigger("Attack");
            
        }

        // 这个方法会被animator的event触发
        public void HandleAttackActionHit()
        {
            if (targetEnemies != null && targetEnemies.Count > 0)
            {
                foreach (var target in targetEnemies)
                {
                    if (target == null) continue; // 检查目标是否已被销毁

                    float distance = Vector2.Distance(transform.position, target.position);
                    if (distance <= attackRange)
                    {
                        Unit enemyUnit = target.GetComponent<Unit>();
                        if (enemyUnit != null)
                        {
                            Attack(enemyUnit, attackPower.finalValue/targetEnemies.Count); // 执行攻击逻辑
                        }
                    }
                }
            }
        }

        private void Attack(Unit target, float damage)
        {
            if (isRanged)
            {
                // 发射投射物
                FireProjectile(target, damage);
            }
            else
            {
                // 近战攻击逻辑
                target.TakeHit(damage, attackEffects);
            }
        }

        private void FireProjectile(Unit target, float damage)
        {
            if (projectilePrefab != null && projectileSpawnPoint != null)
            {
                GameObject projectileObject = Instantiate(projectilePrefab, projectileSpawnPoint.position, transform.rotation);
                Projectiles.Projectile projectile = projectileObject.GetComponent<Projectiles.Projectile>();
                if (projectile != null)
                {
                    projectile.target = target.transform;
                    projectile.damage = damage;
                    projectile.debuffs = new List<Buff>(attackEffects); // 传递攻击效果作为Debuff
                }
            }
        }



        public virtual void TakeHit(float damageReceived, List<Buff> buffs)
        {
            foreach (var buff in buffs)
            {
                AddBuff(buff); // 添加Debuff到自己身上
            }
            TakeDamage(damageReceived); // 扣除生命值
        }

        public void TakeDamage(float damageReceived)
        {
            currentCore -= damageReceived;
            OnDamageTaken?.Invoke(transform, damageReceived); // 触发伤害事件
            healthBar.UpdateHealthBar(currentCore, maxCore.finalValue); // Use GetMaxHP() instead of maxHP
            CheckHealth();
        }

        private void CheckHealth()
        {
            if (currentCore <= 0)
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
            this.faction = faction;
            bodySpriteRenderer.color = faction == Faction.FactionA ? Color.blue : Color.red;
        }

        
    }
}
