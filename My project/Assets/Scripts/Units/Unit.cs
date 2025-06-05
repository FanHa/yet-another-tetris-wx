using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Controller;
using Model.Tetri;
using TMPro;
using UI;
using Units.Buffs;
using Units.Skills;
using UnityEngine;

namespace Units
{
    [RequireComponent(typeof(DotHandler))]
    [RequireComponent(typeof(Units.Skills.SkillHandler))]
    public class Unit : MonoBehaviour
    {
        public Attributes Attributes;
        public Units.Buffs.Manager BuffManager;// Buff管理器
        private Movement movementController;
        [SerializeField] private Units.Skills.SkillHandler skillHandler; // 技能处理器
        private AnimationController animationController;
        public VisualEffectConfig VisualEffectConfig;
        public Model.ProjectileConfig ProjectileConfig;

        public enum Faction
        {
            FactionA,
            FactionB
        }
        [SerializeField] private Color factionAColor;
        [SerializeField] private Color factionBColor;

        public event Action<Unit> OnDeath;

        public event Action<Damages.Damage> OnDamageTaken;
        public event Action<Damages.Damage> OnAttackHit;

        public event Action<Units.Unit, Skill> OnSkillCast;

        public Faction faction; // 单位的阵营
        protected float lastAttackTime = 0;

        private List<ITakeDamageBehavior> damageBehaviors = new List<ITakeDamageBehavior>(); // 伤害行为链

        private HealthBar healthBar;

        public SpriteRenderer BodySpriteRenderer;
        public SpriteRenderer Fist1SpriteRenderer;
        public SpriteRenderer Fist2SpriteRenderer;
        private HitEffect hitEffect;

        private DotHandler dotHandler; // 持续伤害处理器
        public Transform projectileSpawnPoint; // 投射物生成位置

        public List<Unit> enemyUnits = new(); // todo 改成更清晰的名字sortedByDistance

        public UnitManager unitManager;

        public List<Buffs.Buff> attackEffects = new List<Buffs.Buff>(); // 攻击效果列表
        private bool isActive = false; // 是否处于活动状态
        public bool moveable = true;
        public bool isFrozen = false;
        private void Awake()
        {
            animationController = GetComponent<AnimationController>();
            if (animationController == null)
            {
                Debug.LogWarning("AnimationController component not found on the same GameObject.");
            }

            healthBar = GetComponentInChildren<HealthBar>();
            hitEffect = GetComponent<HitEffect>();
            BuffManager = GetComponent<Units.Buffs.Manager>();
            movementController = GetComponent<Movement>();
            skillHandler = GetComponent<Units.Skills.SkillHandler>();
            dotHandler = GetComponent<DotHandler>();

        }

        void Start()
        {
            if (skillHandler != null)
            {
                skillHandler.OnSkillReady += OnSkillReady;
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (isFrozen) return; // 如果被冻结，直接返回
            if (isActive)
            {
                AttackEnemies();
            }
        }

        void FixedUpdate()
        {
            if (isActive)
            {
                // 如果有目标敌人，移动到最近的敌人
                if (enemyUnits != null && enemyUnits.Count > 0)
                {
                    Transform closestEnemy = enemyUnits[0].transform;
                    movementController.MoveTowardsEnemy(closestEnemy);
                }
            }
        }

        public void Initialize()
        {
            Attributes.OnHealthChanged += UpdateHealthBar;
            Attributes.CurrentHealth = Attributes.MaxHealth.finalValue;
            lastAttackTime = Time.time - (10f / Attributes.AttacksPerTenSeconds.finalValue); // 初始化冷却时间

            InvokeRepeating(nameof(BuffEffect), 1f, 1f);
            InvokeRepeating(nameof(UpdateEnemiesDistance), 0f, 0.5f);
            movementController.Initialize(Attributes); // 将 Attributes 传递给 Movement
            skillHandler.Activate(); // 初始化技能管理器
            isActive = true;
        }
        private void UpdateHealthBar(float currentHealth, float maxHealth)
        {
            healthBar.UpdateHealthBar(currentHealth, maxHealth);
        }
        public void SetBattlefieldBounds(Transform minBounds, Transform maxBounds)
        {
            movementController.SetBattlefieldBounds(minBounds, maxBounds);
        }

        public void AddSkill(Skills.Skill newSkill)
        {
            skillHandler.AddSkill(newSkill); // 添加技能
        }
        private void OnSkillReady(Skill skill)
        {
            animationController.TriggerCastSkill();
        }


        // 这个方法会被 Animator 的事件触发
        public void HandleSkillCastAction()
        {
            OnSkillCast?.Invoke(this, skillHandler.GetCurrentSkill());
            skillHandler.ExecutePendingSkill(); // 执行待处理的技能
            
        }

        public void ApplyDot(Dot dot)
        {
            dotHandler.ApplyDot(dot);
        }

        public void StopAction()
        {
            CancelInvoke(nameof(BuffEffect));
            CancelInvoke(nameof(UpdateEnemiesDistance));
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
            BuffManager.AddBuff(buff, this);
        }

        private void BuffEffect()
        {
            BuffManager.UpdateBuffs(this);

        }

        private void UpdateEnemiesDistance()
        {
            List<Unit> rawEnemyUnits = faction == Faction.FactionA ? unitManager.GetFactionBUnits() : unitManager.GetFactionAUnits();

            // 按距离从小到大排序
            enemyUnits = rawEnemyUnits
                .OrderBy(unit => Vector2.SqrMagnitude(unit.transform.position - transform.position)) // 按平方距离排序
                .ToList();
        }

        protected void AttackEnemies()
        {
            float attackCooldown = 10f / Attributes.AttacksPerTenSeconds.finalValue;
            if (Time.time < lastAttackTime + attackCooldown)
                return; // 检查攻击冷却时间
            if (enemyUnits != null && enemyUnits.Count > 0)
            {
                Transform closestEnemy = enemyUnits[0].transform; // 获取最近的敌人
                if (closestEnemy != null)
                {
                    float distance = Vector2.Distance(transform.position, closestEnemy.position);
                    if (distance <= Attributes.AttackRange) // 检查最近的敌人是否在攻击范围内
                    {
                        // 调整朝向
                        Vector2 direction = (closestEnemy.position - transform.position).normalized;
                        animationController.SetLookDirection(direction);

                        TriggerAttack();
                        lastAttackTime = Time.time; // 更新攻击时间
                    }
                }
            }
        }

        private void TriggerAttack()
        {
            animationController.TriggerAttack();
        }

        // 这个方法会被animator的event触发
        public void HandleAttackActionHit()
        {
            int attackTargetCount = Mathf.Min(enemyUnits.Count, (int)Attributes.AttackTargetNumber);

            for (int i = 0; i < attackTargetCount; i++)
            {
                Unit target = enemyUnits[i];
                if (target == null)
                    continue; // todo 这里其实要检查目标是否还活着
                float distance = Vector2.Distance(transform.position, target.transform.position);
                if (distance <= Attributes.AttackRange)
                {
                    Attack(target, Attributes.AttackPower.finalValue / attackTargetCount); // 执行攻击逻辑
                }

            }
        }

        private void Attack(Unit target, float damageValue)
        {
            if (Attributes.IsRanged)
            {
                damageValue = Attributes.RangeAttackDamagePercentage * damageValue / 100;
                Damages.Damage damage = new Damages.Damage(damageValue, Damages.DamageType.Hit);
                damage.SetSourceLabel("远程攻击");
                damage.SetSourceUnit(this);
                damage.SetTargetUnit(target);
                damage.SetBuffs(attackEffects);
                FireProjectile(target, damage);
            }
            else
            {
                Damages.Damage damage = new Damages.Damage(damageValue, Damages.DamageType.Hit);
                damage.SetSourceLabel("近战攻击");
                damage.SetSourceUnit(this);
                damage.SetTargetUnit(target);
                damage.SetBuffs(attackEffects);
                target.TakeDamage(damage);
                OnAttackHit?.Invoke(damage);
            }
        }

        public void TriggerOnAttackHit(Damages.Damage damage)
        {
            OnAttackHit?.Invoke(damage); // 触发攻击命中事件
        }

        public void FireProjectile(Unit target, Damages.Damage damage)
        {
            if (ProjectileConfig.BaseProjectilePrefab != null && projectileSpawnPoint != null)
            {
                GameObject projectileObject = Instantiate(ProjectileConfig.BaseProjectilePrefab, projectileSpawnPoint.position, transform.rotation);
                Projectiles.Projectile projectile = projectileObject.GetComponent<Projectiles.Projectile>();

                if (projectile != null)
                {
                    projectile.Init(this, target.transform, damage);

                    SpriteRenderer projectileSpriteRenderer = projectileObject.GetComponent<SpriteRenderer>();
                    if (projectileSpriteRenderer != null && Fist1SpriteRenderer != null)
                    {
                        projectileSpriteRenderer.sprite = Fist1SpriteRenderer.sprite;
                    }
                    projectileObject.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);

                }
            }
        }



        public void TakeDamage(Units.Damages.Damage damageReceived)
        {
            foreach (Buff buff in damageReceived.Buffs)
            {
                AddBuff(buff); // 添加Debuff到自己身上
            }
            foreach (var behavior in damageBehaviors)
            {
                damageReceived = behavior.ModifyDamage(damageReceived);
            }

            float finalDamage = Mathf.Max(1, Mathf.Round(damageReceived.Value));

            Attributes.CurrentHealth -= finalDamage;

            OnDamageTaken?.Invoke(damageReceived); // 触发伤害事件
            hitEffect.PlayAll();
            CheckHealth();
        }


        private void CheckHealth()
        {
            if (Attributes.CurrentHealth <= 0)
            {
                StopAction();
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
            BodySpriteRenderer.color = color;
            Fist1SpriteRenderer.color = color;
            Fist2SpriteRenderer.color = color;
        }


    }
}
