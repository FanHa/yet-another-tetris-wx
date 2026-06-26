using System;
using System.Collections.Generic;
using System.Linq;
using Controller;
using Model.Tetri;
using UI;
using Units.Actions;
using Units.Buffs;
using Units.Skills;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Units
{
    [RequireComponent(typeof(Units.Skills.SkillHandler))]
    [RequireComponent(typeof(BuffHandler))]
    [RequireComponent(typeof(AnimationController))]
    [RequireComponent(typeof(FacingController))]
    [RequireComponent(typeof(HitEffect))]
    public class Unit : MonoBehaviour, IPointerClickHandler
    {
        public Attributes Attributes;
        private Units.Buffs.BuffHandler buffHandler;// Buff管理器
        private Movement movementController;
        private Units.Skills.SkillHandler skillHandler; // 技能处理器
        private AnimationController animationController;
        private FacingController facingController;
        public Model.ProjectileConfig ProjectileConfig;
        public Dictionary<AffinityType, int> CellCounts = new();

        public enum Faction
        {
            FactionA,
            FactionB
        }

        public enum MoveBehaviorMode
        {
            TowardEnemy,
            TowardAlly
        }

        [SerializeField] private Color factionAColor;
        [SerializeField] private Color factionBColor;
        [SerializeField] private MoveBehaviorMode moveBehaviorMode = MoveBehaviorMode.TowardEnemy;

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
        private int skillMotionLockCount = 0;
        private int stunLockCount = 0;
        public bool IsActive => isActive;
        public bool IsSkillMotionActive => skillMotionLockCount > 0;
        public bool IsStunned => stunLockCount > 0;
        internal Movement Movement => movementController;
        internal Units.Skills.SkillHandler SkillHandler => skillHandler;
        public MoveBehaviorMode CurrentMoveBehaviorMode => moveBehaviorMode;

        private UnitActionRunner actionRunner;

        private void Awake()
        {
            animationController = GetComponent<AnimationController>();
            facingController = GetComponent<FacingController>();
            hitEffect = GetComponent<HitEffect>();
            buffHandler = GetComponent<Units.Buffs.BuffHandler>();
            movementController = GetComponent<Movement>();
            skillHandler = GetComponent<Units.Skills.SkillHandler>();

            actionRunner = new UnitActionRunner(this);
            actionRunner.OnActionStarted += HandleActionStarted;
            actionRunner.OnActionCanceled += HandleActionStopped;
            actionRunner.OnActionInterrupted += HandleActionStopped;

            buffHandler.BuffAdded += (buff) => BuffAdded?.Invoke(buff);
            buffHandler.BuffRemoved += (buff) => BuffRemoved?.Invoke(buff);
            
        }

        // Update is called once per frame
        void Update()
        {
            if (!isActive)
                return;

            UpdateEnemiesDistance();
            UpdateFacing();

            actionRunner.Tick(new global::Units.Actions.ActionTickContext(
                Time.deltaTime,
                GetActionTimelineSpeed()));
            UpdateActionAnimationSpeed();
            
        }

        private void UpdateFacing()
        {
            if (TryGetClosestEnemy(out var enemy))
            {
                Vector2 direction = ((Vector2)enemy.transform.position - (Vector2)transform.position).normalized;
                facingController.FaceTowards(direction);
            }
        }

        public void Setup(Unit.Faction faction)
        {
            SetFaction(faction);

            healthBar.SetAttributes(Attributes);
            UpdateActionAnimationSpeed();
            lastAttackTime = Time.time - (10f / Attributes.AttacksPerTenSeconds.finalValue);
        }

        public void Activate()
        {
            movementController.Initialize(Attributes);
            skillHandler.Activate();
            skillHandler.OnSkillCast += HandleSelfSkillCast;
            UnitManager.OnGlobalSkillCast += HandleGlobalSkillCast;
            healthBar.gameObject.SetActive(true);
            hitEffect.Initialize();
            isActive = true;
        }

        public void Deactivate()
        {
            healthBar.gameObject.SetActive(false);
            UnitManager.OnGlobalSkillCast -= HandleGlobalSkillCast;
            skillHandler.OnSkillCast -= HandleSelfSkillCast;
            skillHandler.Deactivate(); // 如有需要
            buffHandler.RequestRemoveAllActiveBuffs(); // 批量请求移除所有Buff（实际提交在BuffHandler.Update中）
            actionRunner.OnOwnerDeactivated();
            animationController.ResetPlaybackSpeed();
            // 若单位在技能位移中途被回收，需恢复被覆盖的避让优先级，避免残留脏状态
            if (skillMotionLockCount > 0)
            {
                movementController.PopAvoidancePriorityOverride();
            }
            skillMotionLockCount = 0;
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

        public void EnterStun()
        {
            stunLockCount++;
        }

        public void ExitStun()
        {
            if (stunLockCount <= 0)
            {
                stunLockCount = 0;
                return;
            }

            stunLockCount--;
        }

        public void Teleport(Vector3 position)
        {
            ApplyMovement(Movement.MovementRequest.Teleport(position));
        }

        public void EnterSkillMotion(int avoidancePriority)
        {
            skillMotionLockCount++;

            if (skillMotionLockCount == 1)
            {
                movementController.PushAvoidancePriorityOverride(avoidancePriority);
                movementController.ClearNavigationPath();
            }
        }

        public void ClearNavigationPathForSkillMotion()
        {
            movementController.ClearNavigationPath();
        }

        public void ExitSkillMotion()
        {
            if (skillMotionLockCount <= 0)
            {
                skillMotionLockCount = 0;
                return;
            }

            skillMotionLockCount--;

            if (skillMotionLockCount == 0)
            {
                movementController.PopAvoidancePriorityOverride();
            }
        }

        // ============ 位移请求统一入口 ============

        /// <summary>
        /// 应用位移。所有位移操作必须通过 Unit 的这个入口。同步执行，立即返回结果。
        /// </summary>
        public Movement.MovementResult ApplyMovement(Movement.MovementRequest request)
        {
            return movementController.ApplyMovement(request);
        }

        // ============ 动画编排系统（Animation Orchestration） ============
        // 
        // 职责: 处理所有与 AnimationController 的交互，集中管理动画命令的下发和监听。
        //       确保 Unit 是 Actions 与 AnimationController 之间的唯一协调者。
        //
        // 架构流程:
        //   1. ActionRunner 发出事件 (OnActionStarted/Completed/Canceled/Interrupted)
        //   2. Unit 的 HandleAction* 方法响应事件，调用 AnimationController 播放/停止动画
        //   3. Actions 通过 GetAttackActionDurationSeconds()/GetSkillActionDurationSeconds() 获取时间基准
        //   4. Unit.Update() 每帧调用 UpdateActionAnimationSpeed() 更新速度
        //
        // 关键原则:
        //   - 事件驱动: 仅在 Action 生命周期边界（Start/Stop）下发动画命令
        //   - 查询分离: Actions 通过时间线进度驱动，不直接读取 Animator 状态
        //   - 速度管理: 动画播放速度由 Buff/Attributes 决定，每帧同步更新

        /// <summary>
        /// 响应 ActionRunner 事件：Action 启动时，根据类型播放对应动画。
        /// 由 Unit.Awake() 中的事件订阅自动触发，外部不应调用。
        /// </summary>
        private void HandleActionStarted(UnitActionType actionType)
        {
            switch (actionType)
            {
                case UnitActionType.Attack:
                    animationController.PlayAttack();
                    break;
                case UnitActionType.CastSkill:
                    animationController.PlayCastSkill();
                    break;
            }
        }

        /// <summary>
        /// 响应 ActionRunner 事件：Action 停止（取消/中断）时，停止对应动画。
        /// 由 Unit.Awake() 中的事件订阅自动触发，外部不应调用。
        /// </summary>
        private void HandleActionStopped(UnitActionType actionType)
        {
            switch (actionType)
            {
                case UnitActionType.Attack:
                case UnitActionType.CastSkill:
                    animationController.StopAction();
                    break;
            }
        }



        // ============ 导航控制（状态管理） ============

        /// <summary>
        /// 暂停导航。用于技能施放或被击晕时停止自动移动。
        /// </summary>
        public void PauseNavigation()
        {
            movementController.PauseNavigation();
        }

        /// <summary>
        /// 恢复导航。恢复被暂停的自动移动。
        /// </summary>
        public void ResumeNavigation()
        {
            movementController.ResumeNavigation();
        }

        /// <summary>
        /// 初始化单位位置（不涉及 NavMesh 寻路，直接设置）。通常在生成单位时使用。
        /// </summary>
        public void PlaceAt(Vector3 position)
        {
            ApplyMovement(Movement.MovementRequest.PlaceAt(position));
        }

        /// <summary>
        /// 获取当前动作速度乘数。从 Attributes 系统读取（包含所有 Buff 修改器）。
        /// 返回值: 1.0 = 标准速度，0.5 = 半速，2.0 = 双速，依此类推。
        /// </summary>
        private float GetCurrentActionSpeed()
        {
            return Attributes?.ActionSpeed?.finalValue ?? 1f;
        }

        /// <summary>
        /// 获取经过钳制的动作速度（最小为 0）。
        /// 供外部（如 ActionRunner）读取当前速度时使用。
        /// </summary>
        public float GetActionTimelineSpeed()
        {
            return Mathf.Max(0f, GetCurrentActionSpeed());
        }

        // ============ 时间参数（Timing） ============
        //
        // 这组方法为 Actions 提供攻击/技能动作的时间基准。
        // Actions 用这些参数初始化 ActionTimelineProgress，推进后由 timeline 自行决定触发时机。

        /// <summary>
        /// 获取攻击动作总时长（秒）。由 AnimationController 提供，作为 AttackAction 的时间线长度。
        /// </summary>
        public float GetAttackActionDurationSeconds()
        {
            return animationController.GetAttackActionDurationSeconds();
        }

        /// <summary>
        /// 获取技能动作总时长（秒）。作为 CastSkillAction 的时间线长度。
        /// </summary>
        public float GetSkillActionDurationSeconds()
        {
            return animationController.GetSkillActionDurationSeconds();
        }

        // ============ 技能系统 API 包装 ============

        /// <summary>
        /// 检查是否有就绪的技能可以施放。
        /// </summary>
        public bool HasReadySkill => skillHandler.HasReadySkill;

        /// <summary>
        /// 执行待决的技能。
        /// </summary>
        public void ExecutePendingSkill()
        {
            skillHandler.ExecutePendingSkill();
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

        public void SetMoveBehaviorMode(MoveBehaviorMode mode)
        {
            moveBehaviorMode = mode;
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

        /// <summary>
        /// 每帧更新动画播放速度。当 Unit 的 ActionSpeed 属性改变时，
        /// 需要同步到 Animator，使动画快进/减速生效。
        /// 
        /// 调用时机: Unit.Update() 和 Unit.Setup() 中调用，确保速度始终同步。
        /// 
        /// 实现原理:
        ///   - GetCurrentActionSpeed() 从 Buff/Attributes 获取最新速度值
        ///   - animationController.SetPlaybackSpeed() 将其应用到 Animator
        /// </summary>
        private void UpdateActionAnimationSpeed()
        {
            animationController.SetPlaybackSpeed(GetCurrentActionSpeed());
        }

        public void AddBuff(Units.Buffs.Buff buff)
        {
            buffHandler.ApplyBuff(buff);
        }

        public void RemoveBuff(Units.Buffs.Buff buff)
        {
            buffHandler.RemoveBuff(buff);
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


        public bool TryGetClosestEnemy(out Unit closestEnemy)
        {
            closestEnemy = enemyUnits.Count > 0 ? enemyUnits[0] : null;
            return closestEnemy != null;
        }

        public bool TryGetClosestAlly(out Unit closestAlly)
        {
            closestAlly = null;
            if (UnitManager == null) return false;

            var allies = UnitManager.GetUnitsByFaction(faction);
            float bestSqr = float.MaxValue;
            Vector2 selfPos = transform.position;

            for (int i = 0; i < allies.Count; i++)
            {
                var ally = allies[i];
                if (ally == null || !ally.IsActive || ally == this) continue;

                float d2 = ((Vector2)ally.transform.position - selfPos).sqrMagnitude;
                if (d2 < bestSqr)
                {
                    bestSqr = d2;
                    closestAlly = ally;
                }
            }

            return closestAlly != null;
        }

        // 与 AttackAction.CanStart() 使用相同的有效射程（含双方 AgentRadius）
        public bool TryGetClosestEnemyInAttackRange(out Unit closestEnemy)
        {
            closestEnemy = null;
            if (!TryGetClosestEnemy(out var candidate)) return false;
            float distance = Vector2.Distance(transform.position, candidate.transform.position);
            if (distance > GetEffectiveAttackRangeTo(candidate)) return false;
            closestEnemy = candidate;
            return true;
        }

        public bool CanAttackNow()
        {
            float attackCooldown = 10f / Attributes.AttacksPerTenSeconds.finalValue;
            return Time.time >= lastAttackTime + attackCooldown;
        }

        public float BodyRadius => movementController.AgentRadius;

        public float GetEffectiveAttackRangeTo(Unit target)
        {
            return Attributes.AttackRange.finalValue + BodyRadius + target.BodyRadius;
        }

        public void MarkAttackExecuted()
        {
            lastAttackTime = Time.time;
        }

        public void ExecuteAttackProjectile(Unit target, float damageValue)
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
                Deactivate();
                animationController.PlayDeath();
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
        /// </summary>
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
