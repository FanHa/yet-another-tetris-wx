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
using UnityEngine.EventSystems;

namespace Units
{
    [RequireComponent(typeof(DotHandler))]
    [RequireComponent(typeof(Units.Skills.SkillHandler))]
    [RequireComponent(typeof(BuffHandler))]
    public class Unit : MonoBehaviour, IPointerClickHandler
    {
        public Attributes Attributes;
        private Units.Buffs.BuffHandler BuffHandler;// Buff管理器
        private Movement movementController;
        private Units.Skills.SkillHandler skillHandler; // 技能处理器
        private AnimationController animationController;
        public VisualEffectConfig VisualEffectConfig;
        public Model.ProjectileConfig ProjectileConfig;
        public Dictionary<AffinityType, int> CellCounts = new();

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
        public event Action<Unit> OnClicked;
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

        public UnitManager UnitManager;

        // public List<Buffs.Buff> attackEffects = new List<Buffs.Buff>(); // 攻击效果列表
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
            BuffHandler = GetComponent<Units.Buffs.BuffHandler>();
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

        public void Setup()
        {
            Attributes.OnHealthChanged += UpdateHealthBar;
            Attributes.CurrentHealth = Attributes.MaxHealth.finalValue;
            lastAttackTime = Time.time - (10f / Attributes.AttacksPerTenSeconds.finalValue);
            movementController.Initialize(Attributes);
            skillHandler.Initialize(Attributes);
        }

        public void Activate()
        {
            InvokeRepeating(nameof(UpdateEnemiesDistance), 0f, 0.5f);
            skillHandler.Activate();
            isActive = true;
        }

        public void Deactivate()
        {
            CancelInvoke(nameof(UpdateEnemiesDistance));
            skillHandler.Deactivate(); // 如有需要
            isActive = false;
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
        public IReadOnlyList<Skill> GetSkills()
        {
            return skillHandler.GetSkills();
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
            BuffHandler.ApplyBuff(buff);
        }

        private void UpdateEnemiesDistance()
        {
            List<Unit> rawEnemyUnits = faction == Faction.FactionA ? UnitManager.GetFactionBUnits() : UnitManager.GetFactionAUnits();

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

                        animationController.TriggerAttack();
                        lastAttackTime = Time.time; // 更新攻击时间
                    }
                }
            }
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
            GameObject projectileObject;
            if (Attributes.IsRanged)
            {
                projectileObject = Instantiate(ProjectileConfig.BaseProjectilePrefab, projectileSpawnPoint.position, transform.rotation);
            }
            else
            {
                projectileObject = Instantiate(ProjectileConfig.MeleeProjectilePrefab, projectileSpawnPoint.position, transform.rotation);
            }
            Damages.Damage damage = new Damages.Damage(damageValue, Damages.DamageType.Hit);
            damage.SetSourceLabel("普通攻击");
            damage.SetSourceUnit(this);
            damage.SetTargetUnit(target);
            Projectiles.Projectile projectile = projectileObject.GetComponent<Projectiles.Projectile>();
            projectile.SetSprite(Fist1SpriteRenderer.sprite);
            projectile.Init(this, target.transform, damage);
            projectileObject.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
            projectile.Activate();

        }

        public void TriggerAttackHit(Unit target, Damages.Damage damage)
        {
            foreach (var buff in BuffHandler.GetActiveBuffs())
            {
                if (buff is IAttackHitTrigger attackBuff)
                {
                    attackBuff.OnAttackHit(this, target, ref damage);
                }
            }
        }

        public void TakeHit(Unit attacker, ref Damages.Damage damage)
        {
            foreach (var buff in BuffHandler.GetActiveBuffs())
            {
                if (buff is ITakeHitTrigger hitTrigger)
                {
                    hitTrigger.OnTakeHit(this, attacker, ref damage);
                }
            }
            // 处理被攻击逻辑
            TakeDamage(damage);
        }

        public void TakeDamage(Units.Damages.Damage damageReceived)
        {
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
                OnDeath?.Invoke(this);
                // gameObject.SetActive(false); // 将 GameObject 设置为失活
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

        public void OnPointerClick(PointerEventData eventData)
        {
            OnClicked?.Invoke(this);
        }
    }
}
