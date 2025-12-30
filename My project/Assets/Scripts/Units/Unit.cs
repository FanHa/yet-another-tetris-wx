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
        private Units.Buffs.BuffHandler buffHandler;// Buff管理器
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

        [SerializeField] private HealthBar healthBar;

        public SpriteRenderer BodySpriteRenderer;
        public SpriteRenderer Fist1SpriteRenderer;
        public SpriteRenderer Fist2SpriteRenderer;
        private HitEffect hitEffect;

        public Transform projectileSpawnPoint; // 投射物生成位置

        private List<Unit> enemyUnits = new(); // todo 改成更清晰的名字sortedByDistance

        [Header("运行时注入")]
        public UnitManager UnitManager;

        private bool isActive = false; // 是否处于活动状态
        public bool IsActive => isActive;
        private bool isInAction = false;
        private Unit pendingAttackTarget;

        private void Awake()
        {
            animationController = GetComponent<AnimationController>();
            hitEffect = GetComponent<HitEffect>();
            buffHandler = GetComponent<Units.Buffs.BuffHandler>();
            movementController = GetComponent<Movement>();
            skillHandler = GetComponent<Units.Skills.SkillHandler>();

            buffHandler.BuffRemoved += (buff) => BuffRemoved?.Invoke(buff);
            
        }

        // Update is called once per frame
        void Update()
        {
            if (!isActive)
                return;
            UpdateEnemiesDistance();
            ProcessAttack();
            ProcessSkill();
            if (!isInAction && enemyUnits != null && enemyUnits.Count > 0)
            {
                Transform closestEnemy = enemyUnits[0].transform;
                movementController.ExecuteMove(closestEnemy);
            }
            
        }

        public void Setup(Unit.Faction faction, Transform minBounds, Transform maxBounds)
        {
            SetFaction(faction);
            SetBattlefieldBounds(minBounds, maxBounds);

            healthBar.SetAttributes(Attributes);
            lastAttackTime = Time.time - (10f / Attributes.AttacksPerTenSeconds.finalValue);
        }

        public void Activate()
        {
            skillHandler.Activate();
            skillHandler.OnSkillCast += HandleSelfSkillCast;
            UnitManager.OnGlobalSkillCast += HandleGlobalSkillCast;
            isActive = true;
            healthBar.gameObject.SetActive(true);
            hitEffect.Initialize();
            movementController.Initialize(Attributes, UnitManager, this);
        }

        public void Deactivate()
        {
            healthBar.gameObject.SetActive(false);
            UnitManager.OnGlobalSkillCast -= HandleGlobalSkillCast;
            skillHandler.OnSkillCast -= HandleSelfSkillCast;
            skillHandler.Deactivate(); // 如有需要
            buffHandler.GetActiveBuffs().ToList().ForEach(buff => buffHandler.RemoveBuff(buff)); // 清理所有Buff
            isActive = false;
        }

        private void HandleGlobalSkillCast(Unit caster, Skill skill)
        {
            foreach (var buff in buffHandler.GetActiveBuffs())
            {
                if (buff is IGlobalSkillCastTrigger t)
                    t.OnGlobalSkillCast(caster, skill);
            }
        }

        private void HandleSelfSkillCast(Unit unit, Skill skill)
        {
            OnSkillCast?.Invoke(unit, skill);
        }

        public void SetHitAndRun(bool enable)
        {
            movementController.IsHitAndRun = enable;
        }

        public void SetCellAffinity(Dictionary<AffinityType, int> CellCounts)
        {
            this.CellCounts = CellCounts;

            this.CellCounts.TryGetValue(AffinityType.Life, out int lifeCount);
            float hpBonus = 20f * lifeCount;
            Attributes.MaxHealth.AddFlatModifier(this, hpBonus);

            this.CellCounts.TryGetValue(AffinityType.Wind, out int windCount);
            float rangeBonus = 0.25f * windCount;
            Attributes.AttackRange.AddFlatModifier(this, rangeBonus);

            this.CellCounts.TryGetValue(AffinityType.Swift, out int swiftCount);
            float swiftBonus = 0.35f * swiftCount;
            Attributes.MoveSpeed.AddFlatModifier(this, swiftBonus);
        }


        private void SetBattlefieldBounds(Transform minBounds, Transform maxBounds)
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

        public void AddSkillEnergy(float energy)
        {
            skillHandler.DistributeEnergy(energy);
        }

        private void ProcessSkill()
        {
            if (isInAction) return;
            if (skillHandler.HasReadySkill)
            {
                isInAction = true;                  // 锁动作
                animationController.TriggerCastSkill(); // 播放施法动画
            }
        }

        // 这个方法会被 Animator 的事件触发
        public void HandleSkillCastAnimationEnd()
        {
            
            skillHandler.ExecutePendingSkill(); // 执行待处理的技能
            isInAction = false;

        }

        public void AddBuff(Units.Buffs.Buff buff)
        {
            buffHandler.ApplyBuff(buff);
            BuffAdded?.Invoke(buff); // 触发Buff添加事件
        }

        public void RemoveBuff(Units.Buffs.Buff buff)
        {
            buffHandler.RemoveBuff(buff);
            BuffRemoved?.Invoke(buff); // 触发Buff移除事件
        }

        private void UpdateEnemiesDistance()
        {
            if (UnitManager == null) return;

            var list = faction == Faction.FactionA
                ? UnitManager.GetFactionBUnits()
                : UnitManager.GetFactionAUnits();

            Unit closest = null;
            float bestSqr = float.MaxValue;
            Vector2 selfPos = transform.position;

            for (int i = 0; i < list.Count; i++)
            {
                var u = list[i];
                if (u == null || !u.IsActive) continue;

                float d2 = ((Vector2)u.transform.position - selfPos).sqrMagnitude;
                if (d2 < bestSqr)
                {
                    bestSqr = d2;
                    closest = u;
                }
            }

            // 只用到最近一个时，避免维护整张有序表
            enemyUnits.Clear();
            if (closest != null) enemyUnits.Add(closest);
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
                if (distance <= Attributes.AttackRange.finalValue) // 检查最近的敌人是否在攻击范围内
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
            if (distance <= Attributes.AttackRange.finalValue)
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
            projectileObject = Instantiate(ProjectileConfig.RangeAttackProjectilePrefab, projectileSpawnPoint.position, transform.rotation);
            var projectile = projectileObject.GetComponent<Projectiles.RangeAttack>();
            projectile.Init(this, target);
            projectile.Activate();
            

        }

        public void TriggerAttackHit(Unit target, Damages.Damage damage)
        {
            List<Buff> buffs = buffHandler.GetActiveBuffs().ToList(); // 复制一份，避免遍历时修改
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
            List<Buff> buffs = buffHandler.GetActiveBuffs().ToList(); // 复制一份，避免遍历时修改
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
            var buffs = buffHandler.GetActiveBuffs().ToList();

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
                animationController.TriggerDie();
                Deactivate();
                OnDeath?.Invoke(this);
            }
        }



        void OnDrawGizmosSelected()
        {

            // 绘制攻击范围的Gizmos
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, Attributes.AttackRange.finalValue);
        }

        /// <summary>
        /// Sets the faction of the unit and updates the color of the body sprite renderer accordingly.
        /// <param name="faction">The faction to set for the unit.</param>
        private void SetFaction(Faction faction)
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
            return buffHandler.GetActiveBuffs().ToList();
        }        
    }
}
