using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Units.Skills
{
    public class SkillHandler : MonoBehaviour
    {
        public float energyDecayPerSkill;
        private float tickTimer;
        private const float TICK_INTERVAL = 0.2f;
        private bool isActive = false;

        private ISkillRuntimeContext runtimeContext;
        private IUnitSkillContext skillContext;

        public event Action<Skill> OnSkillReady;
        public event Action<SkillQueuedEvent> OnSkillQueued;
        public event Action<SkillCastStartedEvent> OnSkillCastStarted;
        public event Action<SkillCastSucceededEvent> OnSkillCastSucceeded;
        public event Action<SkillCastFailedEvent> OnSkillCastFailed;

        private readonly List<Skill> skills = new();
        private readonly Queue<ActiveSkill> readyQueue = new();
        private readonly HashSet<ActiveSkill> queuedSet = new();

        void Awake()
        {
            ResolveContexts();
        }

        public void Inject(ISkillRuntimeContext runtimeContext, IUnitSkillContext skillContext)
        {
            this.runtimeContext = runtimeContext;
            this.skillContext = skillContext;
        }

        private void ResolveContexts()
        {
            if (runtimeContext != null && skillContext != null)
            {
                return;
            }

            var unit = GetComponent<Unit>();
            runtimeContext ??= unit;
            skillContext ??= unit;
        }


        void Update()
        {
            if (!isActive) return;
            tickTimer += Time.deltaTime;
            if (tickTimer >= TICK_INTERVAL)
            {
                tickTimer -= TICK_INTERVAL;

                float energyPerTick = runtimeContext.Attributes.EnergyPerSecond.finalValue * TICK_INTERVAL;
                DistributeEnergy(energyPerTick);
            }
        }

        public void Activate()
        {
            isActive = true;
            tickTimer = 0f;
        }

        public void Deactivate()
        {
            isActive = false;
            readyQueue.Clear();
            queuedSet.Clear();
        }

        public void AddSkill(Skill newSkill)
        {
            if (skills.Any(skill => skill.GetType() == newSkill.GetType()))
                return;
            newSkill.Owner = skillContext;
            skills.Add(newSkill);
        }

        public IReadOnlyList<Skill> GetSkills()
        {
            return skills;
        }

        public void DistributeEnergy(float baseEnergy)
        {
            var activeSkills = skills.OfType<ActiveSkill>().ToList();
            if (activeSkills.Count == 0) return;

            float decayFactor = Mathf.Pow(energyDecayPerSkill, Mathf.Max(0, activeSkills.Count - 1));
            float gainPerSkill = baseEnergy * decayFactor;

            foreach (var s in activeSkills)
                s.AddEnergy(gainPerSkill);

            // 入队阶段只依赖能量条件，不依赖 IsReady() 返回值。
            // IsReady() 在部分技能里带有“缓存目标”等副作用，这里仅用于预缓存，
            // 真正的施放判定在 TryCastNextSkill() 里做二次校验。
            foreach (var s in activeSkills)
            {
                if (s.CurrentEnergy >= s.RequiredEnergy && queuedSet.Add(s))
                {
                    s.IsReady();
                    readyQueue.Enqueue(s);
                    OnSkillReady?.Invoke(s);
                    OnSkillQueued?.Invoke(new SkillQueuedEvent(skillContext.SelfUnit, s, s.CurrentEnergy, s.RequiredEnergy));
                }
            }
        }


        public SkillCastResult TryCastNextSkill()
        {
            if (readyQueue.Count == 0)
            {
                OnSkillCastFailed?.Invoke(new SkillCastFailedEvent(skillContext.SelfUnit, null, SkillCastFailureReason.NoPendingSkill));
                return SkillCastResult.Failure(null, SkillCastFailureReason.NoPendingSkill);
            }

            // 先出队，避免 Execute() 过程中队列被清空后再次 Dequeue 引发异常
            var skill = readyQueue.Dequeue();
            queuedSet.Remove(skill);

            // 二次校验：入队只代表能量达标，真正施放前需再次确认前置条件。
            if (!skill.IsReady())
            {
                OnSkillCastFailed?.Invoke(new SkillCastFailedEvent(skillContext.SelfUnit, skill, SkillCastFailureReason.PrerequisiteNotMet));
                return SkillCastResult.Failure(skill, SkillCastFailureReason.PrerequisiteNotMet);
            }

            if (!skill.CanExecuteNow())
            {
                OnSkillCastFailed?.Invoke(new SkillCastFailedEvent(skillContext.SelfUnit, skill, SkillCastFailureReason.InvalidTarget));
                return SkillCastResult.Failure(skill, SkillCastFailureReason.InvalidTarget);
            }

            OnSkillCastStarted?.Invoke(new SkillCastStartedEvent(skillContext.SelfUnit, skill));

            bool result = skill.Execute();
            if (result)
            {
                OnSkillCastSucceeded?.Invoke(new SkillCastSucceededEvent(skillContext.SelfUnit, skill));
                return SkillCastResult.Success(skill);
            }

            OnSkillCastFailed?.Invoke(new SkillCastFailedEvent(skillContext.SelfUnit, skill, SkillCastFailureReason.ExecuteCoreFailed));
            return SkillCastResult.Failure(skill, SkillCastFailureReason.ExecuteCoreFailed);
        }

        public bool HasReadySkill => readyQueue.Count > 0;

        public ActiveSkill PeekReadySkill()
        {
            return readyQueue.Count > 0 ? readyQueue.Peek() : null;
        }
    }
}