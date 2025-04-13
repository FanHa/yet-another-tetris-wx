using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Model.Tetri;
using TMPro;
using UI;
using Units.Buffs;
using UnityEngine;

namespace Units
{
    public class Unit : MonoBehaviour
    {
        public Attributes Attributes;

        public enum Faction
        {
            FactionA,
            FactionB
        }
        [SerializeField] private Color factionAColor;
        [SerializeField] private Color factionBColor;

        private Transform battlefieldMinBounds; // 战场的最小边界
        private Transform battlefieldMaxBounds; // 战场的最大边界

        public event Action<Unit> OnDeath;

        public event Action<Damages.EventArgs> OnDamageTaken;


        public event Action<Unit> OnAttacked; // 被攻击事件

        public Faction faction; // 单位的阵营
        protected float lastAttackTime = 0;

        private List<ITakeDamageBehavior> damageBehaviors = new List<ITakeDamageBehavior>(); // 伤害行为链

        public float minDistance = 0.2f; // 与敌人保持的最小距离
        
        private Animator animator;
        private HealthBar healthBar;

        // [SerializeField] private GameObject damageTextPrefab; // 伤害显示的Prefab
        [SerializeField] private SpriteRenderer bodySpriteRenderer;
        [SerializeField] private SpriteRenderer Fist1SpriteRenderer;
        [SerializeField] private SpriteRenderer Fist2SpriteRenderer;
        private HitEffect hitEffect;
        public GameObject projectilePrefab; // 投射物预制体
        public GameObject bombPrefab; // TODO 暂时所有projectile的prefab都放到这里,以后再改
        public GameObject PrecisionArrowPrefab;
        public Transform projectileSpawnPoint; // 投射物生成位置
        public List<Transform> targetEnemies;
        private Transform factionAParent;
        private Transform factionBParent;

        public List<Buffs.Buff> attackEffects = new List<Buffs.Buff>(); // 攻击效果列表
        private List<Skills.Skill> _skills = new List<Skills.Skill>(); // 私有字段

        [SerializeField] private BuffViewer buffViewerPrefab; // Buff预制体
        [SerializeField] private Transform buffViewerParent; 
        [SerializeField] private Dictionary<string, Buffs.Buff> activeBuffs = new Dictionary<string, Buffs.Buff>();
        [SerializeField] private TetriCellTypeResourceMapping tetriCellTypeResourceMapping;
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
            hitEffect = GetComponent<HitEffect>();
            Attributes.OnHealthChanged += UpdateHealthBar;
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
            ClampPositionToBattlefield();
        }

        public void Initialized()
        {
            InvokeRepeating(nameof(BuffEffect), 1f, 1f);
            InvokeRepeating(nameof(FindClosestEnemies), 0f, 0.1f);

            foreach (Units.Skills.Skill skill in _skills)
            {
                skill.Init();
            }
            InvokeRepeating(nameof(CastSkills) , 0f, 0.2f); // 每秒调用一次技能
            isActive = true;
        }
        private void UpdateHealthBar(float currentHealth, float maxHealth)
        {
            healthBar.UpdateHealthBar(currentHealth, maxHealth);
        }
        public void SetBattlefieldBounds(Transform minBounds, Transform maxBounds)
        {
            battlefieldMinBounds = minBounds;
            battlefieldMaxBounds = maxBounds;
        }

        /// <summary>
        /// 限制单位位置在战场边界内
        /// </summary>
        private void ClampPositionToBattlefield()
        {
            if (battlefieldMinBounds == null || battlefieldMaxBounds == null)
            {
                Debug.LogWarning("Battlefield bounds are not set for this unit.");
                return;
            }

            Vector3 clampedPosition = transform.position;
            clampedPosition.x = Mathf.Clamp(clampedPosition.x, battlefieldMinBounds.position.x, battlefieldMaxBounds.position.x);
            clampedPosition.y = Mathf.Clamp(clampedPosition.y, battlefieldMinBounds.position.y, battlefieldMaxBounds.position.y);
            transform.position = clampedPosition;
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

        public void AddDamageBehavior(ITakeDamageBehavior behavior)
        {
            damageBehaviors.Add(behavior);
        }

        public void RemoveDamageBehavior(ITakeDamageBehavior behavior)
        {
            damageBehaviors.Remove(behavior);
        }


        public void AddBuff(Units.Buffs.Buff buff)
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
                BuffViewer buffViewerInstance = Instantiate(buffViewerPrefab, buffViewerParent.transform);
                buffViewerInstance.name = buff.Name(); // 设置名字为 Buff 的类型名
                buffViewerInstance.SetBuffSprite(tetriCellTypeResourceMapping.GetSprite(buff.TetriCellType)); // 设置图标
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
                    // 直接通过子对象名字找到对应的 BuffViewer 并销毁
                    Transform child = buffViewerParent.transform.Find(buff.Name());
                    if (child != null)
                    {
                        Destroy(child.gameObject);
                    }
                    
                } else {
                    buff.Affect(this);
                } 
            }

            // 注: 不能直接在dictionary循环里里Remove,否则会报错,所以先记录后删除
            foreach (var buffKey in expiredBuffs)
            {
                activeBuffs.Remove(buffKey);
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
                .Take((int)Attributes.AttackTargetNumber)
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

                    if (distance > Attributes.AttackRange && distance > minDistance)
                    {
                        // 调整自己的方向
                        Vector2 direction = (closestEnemy.position - transform.position).normalized;

                        // 检查与友方单位的距离，避免扎堆
                        Vector2 adjustedDirection = AdjustDirectionToAvoidAllies(direction);

                        // 计算下一步位置
                        Vector2 newPosition = Vector2.MoveTowards(transform.position, (Vector2)transform.position + adjustedDirection, Attributes.MoveSpeed.finalValue * Time.deltaTime);

                        // 更新位置
                        transform.position = newPosition;

                        // 调整朝向
                        AdjustLookDirection(adjustedDirection);
                    }
                }
            }
        }

        private Vector2 AdjustDirectionToAvoidAllies(Vector2 originalDirection)
        {
            // 获取所有友方单位
            Collider2D[] nearbyUnits = Physics2D.OverlapCircleAll(transform.position, minDistance);

            Vector2 avoidanceVector = Vector2.zero;
            int avoidanceCount = 0;

            foreach (var collider in nearbyUnits)
            {
                if (collider.gameObject != gameObject && collider.TryGetComponent<Unit>(out Unit otherUnit))
                {
                    // 检查是否为友方单位
                    if (otherUnit.faction == faction)
                    {
                        // 计算与友方单位的方向
                        Vector2 toOtherUnit = (Vector2)(transform.position - otherUnit.transform.position);

                        // 累加避让方向
                        avoidanceVector += toOtherUnit.normalized;
                        avoidanceCount++;
                    }
                }
            }

            if (avoidanceCount > 0)
            {
                // 平均避让方向
                avoidanceVector /= avoidanceCount;

                // 调整原始方向，加入避让向量
                return (originalDirection + avoidanceVector).normalized;
            }

            // 如果没有需要避让的单位，返回原始方向
            return originalDirection;
        }

        private void AdjustLookDirection(Vector2 direction)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; // 计算角度
            transform.rotation = Quaternion.Euler(0, 0, angle-90f);//设置自身旋转
        }

        protected void AttackEnemies()
        {
            float attackCooldown = 10f / Attributes.AttacksPerTenSeconds.finalValue;
            if (Time.time < lastAttackTime + attackCooldown) 
                return; // 检查攻击冷却时间
            if (targetEnemies != null && targetEnemies.Count > 0)
            {
                Transform closestEnemy = targetEnemies[0]; // 获取最近的敌人
                if (closestEnemy != null)
                {
                    float distance = Vector2.Distance(transform.position, closestEnemy.position);
                    if (distance <= Attributes.AttackRange) // 检查最近的敌人是否在攻击范围内
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
                    if (distance <= Attributes.AttackRange)
                    {
                        Unit enemyUnit = target.GetComponent<Unit>();
                        if (enemyUnit != null)
                        {
                            Attack(enemyUnit, Attributes.AttackPower.finalValue/targetEnemies.Count); // 执行攻击逻辑
                        }
                    }
                }
            }
        }

        private void Attack(Unit target, float damage)
        {
            if (Attributes.IsRanged)
            {
                // 发射投射物
                FireProjectile(target, damage);
            }
            else
            {
                // 近战攻击逻辑
                target.TakeHit(this, new Damages.Damage(damage, "近战攻击", true), attackEffects);
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
                    projectile.damage = new Damages.Damage(damage, "远程攻击", true); // 设置投射物的伤害
                    projectile.debuffs = new List<Buffs.Buff>(attackEffects); // 传递攻击效果作为Debuff
                    projectile.caster = this; // 设置投射物的施法者

                    // 设置投射物的 Sprite 为 Fist1SpriteRenderer 的 Sprite
                    SpriteRenderer projectileSpriteRenderer = projectileObject.GetComponent<SpriteRenderer>();
                    if (projectileSpriteRenderer != null && Fist1SpriteRenderer != null)
                    {
                        projectileSpriteRenderer.sprite = Fist1SpriteRenderer.sprite;
                    }
                    projectileObject.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);

                }
            }
        }



        public virtual void TakeHit(Unit source, Units.Damages.Damage  damageReceived, List<Buffs.Buff> buffs)
        {
            foreach (var buff in buffs)
            {
                AddBuff(buff); // 添加Debuff到自己身上
            }
            TakeDamage(source, damageReceived); // 扣除生命值
            hitEffect.PlayAll();
            OnAttacked?.Invoke(source); // 触发被攻击事件
        }

        public void TakeDamage(Unit source, Units.Damages.Damage damageReceived)
        {   
            foreach (var behavior in damageBehaviors)
            {
                damageReceived = behavior.ModifyDamage(source, damageReceived);
            }

            float finalDamage = Mathf.Max(1, Mathf.Round(damageReceived.Value));

            Attributes.CurrentHealth -= finalDamage;
            
            OnDamageTaken?.Invoke(new Damages.EventArgs(source, this, damageReceived));

            CheckHealth();
        }


        private void CheckHealth()
        {
            if (Attributes.CurrentHealth <= 0)
            {
                OnDeath?.Invoke(this);
                gameObject.SetActive(false); // 将 GameObject 设置为失活
            }
        }

        void OnDrawGizmosSelected()
        {

            // 绘制攻击范围的Gizmos
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, Attributes.AttackRange);
        }

        /// <summary>
        /// Sets the faction of the unit and updates the color of the body sprite renderer accordingly.
        /// <param name="faction">The faction to set for the unit.</param>
        public void SetFaction(Faction faction)
        {
            this.faction = faction;
            Color color = faction == Faction.FactionA ? factionAColor : factionBColor;
            bodySpriteRenderer.color = color;
            Fist1SpriteRenderer.color = color;
            Fist2SpriteRenderer.color = color;
        }

        
    }
}
