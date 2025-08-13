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
    [RequireComponent(typeof(Units.Skills.SkillHandler))]
    [RequireComponent(typeof(BuffHandler))]
    public class Unit : MonoBehaviour, IPointerClickHandler
    {
        public Attributes Attributes;
        private Units.Buffs.BuffHandler BuffHandler;// Buff管理器
        private Movement movementController;
        private Units.Skills.SkillHandler skillHandler; // 技能处理器
        private AnimationController animationController;
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
        public event Action<Unit> OnClicked;
        public event Action<Units.Unit, Skill> OnSkillCast;
        public event Action<Buff> BuffAdded;
        public event Action<Buff> BuffRemoved;

        public Faction faction; // 单位的阵营
        protected float lastAttackTime = 0;

        private HealthBar healthBar;

        public SpriteRenderer BodySpriteRenderer;
        public SpriteRenderer Fist1SpriteRenderer;
        public SpriteRenderer Fist2SpriteRenderer;
        private HitEffect hitEffect;

        public Transform projectileSpawnPoint; // 投射物生成位置

        public List<Unit> enemyUnits = new(); // todo 改成更清晰的名字sortedByDistance

        [Header("运行时注入")]
        public UnitManager UnitManager;

        private bool isActive = false; // 是否处于活动状态
        public bool IsActive => isActive;
        private bool isInAction = false;
        private Unit pendingAttackTarget;

        private void Awake()
        {
            animationController = GetComponent<AnimationController>();

            healthBar = GetComponentInChildren<HealthBar>();
            hitEffect = GetComponent<HitEffect>();
            BuffHandler = GetComponent<Units.Buffs.BuffHandler>();
            movementController = GetComponent<Movement>();
            skillHandler = GetComponent<Units.Skills.SkillHandler>();
        }

        void Start()
        {
            skillHandler.OnSkillReady += HandleSkillReady;
        }



        // Update is called once per frame
        void Update()
        {
            if (!isActive)
                return;
            ProcessAttack();
            
        }

        void FixedUpdate()
        {
            if (isActive &&  isInAction == false)
            {
                // 如果有目标敌人，移动到最近的敌人
                if (enemyUnits != null && enemyUnits.Count > 0)
                {
                    Transform closestEnemy = enemyUnits[0].transform;
                    movementController.ExecuteMove(closestEnemy);
                }
            }
        }

        public void Setup()
        {
            healthBar.SetAttributes(Attributes);
            lastAttackTime = Time.time - (10f / Attributes.AttacksPerTenSeconds.finalValue);
            movementController.Initialize(Attributes, UnitManager, this);
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
            BuffHandler.GetActiveBuffs().ToList().ForEach(buff => BuffHandler.RemoveBuff(buff)); // 清理所有Buff
            isActive = false;
        }
        public void SetHitAndRun(bool enable)
        {
            movementController.IsHitAndRun = enable;
        }


        public void SetBattlefieldBounds(Transform minBounds, Transform maxBounds)
        {
            movementController.SetBattlefieldBounds(minBounds, maxBounds);
        }

        public void AddSkill(Skills.Skill newSkill)
        {
            skillHandler.AddSkill(newSkill); // 添加技能
        }
        
        public IReadOnlyList<Skill> GetSkills()
        {
            return skillHandler.GetSkills();
        }

        private void HandleSkillReady(Skill skill)
        {
            if (isInAction)
                return; // 技能动作期间不响应新的技能
            isInAction = true; // 技能动画开始，禁止移动
            animationController.TriggerCastSkill();
        }

        // 这个方法会被 Animator 的事件触发
        public void HandleSkillCastAnimationEnd()
        {
            OnSkillCast?.Invoke(this, skillHandler.GetCurrentSkill());
            skillHandler.ExecutePendingSkill(); // 执行待处理的技能\
            isInAction = false;

        }

        public void AddBuff(Units.Buffs.Buff buff)
        {
            BuffHandler.ApplyBuff(buff);
            BuffAdded?.Invoke(buff); // 触发Buff添加事件
        }

        public void RemoveBuff(Units.Buffs.Buff buff)
        {
            BuffHandler.RemoveBuff(buff);
            BuffRemoved?.Invoke(buff); // 触发Buff移除事件
        }

        private void UpdateEnemiesDistance()
        {
            List<Unit> rawEnemyUnits = faction == Faction.FactionA ? UnitManager.GetFactionBUnits() : UnitManager.GetFactionAUnits();

            // 按距离从小到大排序
            enemyUnits = rawEnemyUnits
                .OrderBy(unit => Vector2.SqrMagnitude(unit.transform.position - transform.position)) // 按平方距离排序
                .ToList();
        }

        protected void ProcessAttack()
        {
            if (isInAction)
                return;
            float attackCooldown = 10f / Attributes.AttacksPerTenSeconds.finalValue;
            if (Time.time < lastAttackTime + attackCooldown)
                return; // 检查攻击冷却时间
            if (enemyUnits != null && enemyUnits.Count > 0)
            {
                Unit target = enemyUnits[0];

                float distance = Vector2.Distance(transform.position, target.transform.position);
                if (distance <= Attributes.AttackRange) // 检查最近的敌人是否在攻击范围内
                {
                    pendingAttackTarget = target;
                    isInAction = true; // 攻击动画开始，禁止移动
                    // 调整朝向
                    Vector2 direction = (target.transform.position - transform.position).normalized;
                    animationController.SetLookDirection(direction);
                    animationController.TriggerAttack();
                    
                }
                
            }
        }

        // 这个方法会被animator的event触发
        public void HandleAttackAnimationEnd()
        {
            if (pendingAttackTarget == null)
                return;

            float distance = Vector2.Distance(transform.position, pendingAttackTarget.transform.position);
            if (distance <= Attributes.AttackRange)
            {
                Attack(pendingAttackTarget, Attributes.AttackPower.finalValue);
            }
            else
            {
                // todo miss 处理
            }
            pendingAttackTarget = null;
            isInAction = false;
            lastAttackTime = Time.time; // 更新攻击时间
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
            List<Buff> buffs = BuffHandler.GetActiveBuffs().ToList(); // 复制一份，避免遍历时修改
            foreach (Buff buff in buffs)
            {
                if (buff is IAttackHitTrigger attackBuff)
                {
                    attackBuff.OnAttackHit(this, target, ref damage);
                }
            }
        }

        public void TakeHit(Unit attacker, ref Damages.Damage damage)
        {
            List<Buff> buffs = BuffHandler.GetActiveBuffs().ToList(); // 复制一份，避免遍历时修改
            foreach (Buff buff in buffs)
            {
                if (buff is ITakeHitTrigger takeHitTrigger)
                {
                    takeHitTrigger.OnTakeHit(this, attacker, ref damage);
                }
            }
            // 处理被攻击逻辑
            TakeDamage(damage);
        }

        public void TakeDamage(Units.Damages.Damage damageReceived)
        {
            var buffs = BuffHandler.GetActiveBuffs().ToList();

            Attributes.TakeDamage(damageReceived.Value);
            foreach (var buff in buffs)
            {
                if (buff is IAfterTakeDamageTrigger trigger)
                {
                    trigger.OnAfterTakeDamage(ref damageReceived);
                }
            }

            OnDamageTaken?.Invoke(damageReceived); // 触发伤害事件
            hitEffect.PlayAll();

            if (Attributes.CurrentHealth <= 0)
            {
                Deactivate();
                OnDeath?.Invoke(this);
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

        public List<Units.Buffs.Buff> GetActiveBuffs()
        {
            return BuffHandler.GetActiveBuffs().ToList();
        }        
    }
}
