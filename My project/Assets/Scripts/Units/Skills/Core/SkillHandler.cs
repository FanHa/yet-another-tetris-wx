using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Units.Skills
{
    public class SkillHandler
    {
        private readonly float energyDecayPerSkill;
        private float tickTimer;
        private const float TICK_INTERVAL = 0.2f;
        private bool isActive = false;

        private readonly ISkillHandlerContext context;

        public event Action<SkillCastStartedEvent> OnSkillCastStarted;
        public event Action<SkillCastSucceededEvent> OnSkillCastSucceeded;
        public event Action<SkillCastFailedEvent> OnSkillCastFailed;

        private readonly List<Skill> skills = new();
        private readonly List<ActiveSkill> activeSkills = new();
        private readonly Queue<ActiveSkill> readyQueue = new();
        private readonly HashSet<ActiveSkill> queuedSet = new();
        public SkillHandler(ISkillHandlerContext context, float energyDecayPerSkill = 1f)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
            if (float.IsNaN(energyDecayPerSkill) || float.IsInfinity(energyDecayPerSkill) || energyDecayPerSkill < 0f || energyDecayPerSkill > 1f)
            {
                throw new ArgumentOutOfRangeException(nameof(energyDecayPerSkill), "Must be finite and between 0 and 1.");
            }

            this.energyDecayPerSkill = energyDecayPerSkill;
        }

        public void Tick(float deltaTime)
        {
            if (!isActive)
            {
                return;
            }

            tickTimer += deltaTime;
            if (tickTimer >= TICK_INTERVAL)
            {
                float elapsedEnergyTime = Mathf.Floor(tickTimer / TICK_INTERVAL) * TICK_INTERVAL;
                tickTimer -= elapsedEnergyTime;

                float energyToDistribute = context.EnergyPerSecond * elapsedEnergyTime;
                DistributeEnergy(energyToDistribute);
            }
        }

        public void Activate()
        {
            isActive = true;
            tickTimer = 0f;
            readyQueue.Clear();
            queuedSet.Clear();
            foreach (var skill in skills)
            {
                skill.OnActivate();
            }
        }

        public void Deactivate()
        {
            isActive = false;
            tickTimer = 0f;
            readyQueue.Clear();
            queuedSet.Clear();
            foreach (var skill in skills)
            {
                skill.OnDeactivate();
            }
        }

        public void OnAssemblyCompleted()
        {
            foreach (Skill skill in skills)
            {
                skill.OnPassiveEffectsApplication();
            }
        }

        public void NotifyUnitDeath()
        {
            foreach (Skill skill in skills.ToArray())
            {
                skill.OnUnitDeath();
            }
        }

        public void Release()
        {
            foreach (var skill in skills)
            {
                skill.OnRelease();
            }
        }



        internal bool HasSkillType(Type skillType)
        {
            return skills.Any(skill => skill.GetType() == skillType);
        }

        internal void AddSkill(Skill newSkill)
        {
            if (HasSkillType(newSkill.GetType()))
                return;
            if (newSkill.OwnerContext == null || newSkill.OwnerContext.Owner != context.Owner)
            {
                throw new InvalidOperationException("A skill must be bound to this handler's owner before registration.");
            }

            skills.Add(newSkill);
            if (newSkill is ActiveSkill activeSkill)
            {
                activeSkills.Add(activeSkill);
            }
        }

        public IReadOnlyList<Skill> GetSkills()
        {
            return skills;
        }

        internal void DistributeEnergy(float baseEnergy)
        {
            if (activeSkills.Count == 0) return;

            float decayFactor = Mathf.Pow(energyDecayPerSkill, Mathf.Max(0, activeSkills.Count - 1));
            float gainPerSkill = baseEnergy * decayFactor;

            foreach (var s in activeSkills)
                s.AddEnergy(gainPerSkill);

            foreach (var s in activeSkills)
            {
                if (!queuedSet.Contains(s) && s.IsReady())
                {
                    queuedSet.Add(s);
                    readyQueue.Enqueue(s);
                }
            }
        }

        internal void ReduceRandomSkillEnergy(float energyAmount)
        {
            // 只允许削减有蓄能、但尚未就绪且未进入释放队列的技能。
            List<ActiveSkill> candidates = new();
            float availableEnergy = 0f;
            foreach (ActiveSkill skill in activeSkills)
            {
                if (skill.HasEnoughEnergy || queuedSet.Contains(skill) || skill.CurrentEnergy <= 0f)
                {
                    continue;
                }

                candidates.Add(skill);
                availableEnergy += skill.CurrentEnergy;
            }

            // 削减预算不能超过所有候选技能当前蓄有的能量总和。
            float remainingEnergy = Mathf.Min(energyAmount, availableEnergy);
            if (remainingEnergy <= 0f)
            {
                return;
            }

            // Fisher-Yates 洗牌，避免技能原有顺序影响后续随机分配。
            for (int index = candidates.Count - 1; index > 0; index--)
            {
                int swapIndex = UnityEngine.Random.Range(0, index + 1);
                ActiveSkill skill = candidates[index];
                candidates[index] = candidates[swapIndex];
                candidates[swapIndex] = skill;
            }

            // 找到承接完整预算所需的最少技能数，防止随机选出的技能蓄能总量不足。
            int minimumSkillCount = 0;
            float selectedCapacity = 0f;
            while (selectedCapacity < remainingEnergy)
            {
                selectedCapacity += candidates[minimumSkillCount].CurrentEnergy;
                minimumSkillCount++;
            }

            // 在最少数量到全部候选之间随机决定本次会影响几个技能。
            int affectedSkillCount = UnityEngine.Random.Range(minimumSkillCount, candidates.Count + 1);
            for (int index = 0; index < affectedSkillCount && remainingEnergy > 0f; index++)
            {
                // 当前技能不能扣得太多，否则会导致后续技能无能量可分配。
                float remainingCapacity = 0f;
                for (int nextIndex = index + 1; nextIndex < affectedSkillCount; nextIndex++)
                {
                    remainingCapacity += candidates[nextIndex].CurrentEnergy;
                }

                float minimumReduction = Mathf.Max(0f, remainingEnergy - remainingCapacity);
                float maximumReduction = Mathf.Min(candidates[index].CurrentEnergy, remainingEnergy);
                // 最后一个技能承接剩余预算，保证总削减量精确用完。
                float reduction = index == affectedSkillCount - 1
                    ? remainingEnergy
                    : UnityEngine.Random.Range(minimumReduction, maximumReduction);
                candidates[index].RemoveEnergy(reduction);
                remainingEnergy -= reduction;
            }
        }


        public SkillCastResult TryCastNextSkill()
        {
            if (readyQueue.Count == 0)
            {
                OnSkillCastFailed?.Invoke(new SkillCastFailedEvent(context.Owner, null, SkillCastFailureReason.NoPendingSkill));
                return SkillCastResult.Failure(null, SkillCastFailureReason.NoPendingSkill);
            }

            // 先出队，避免 Execute() 过程中队列被清空后再次 Dequeue 引发异常
            var skill = readyQueue.Dequeue();
            queuedSet.Remove(skill);

            if (!skill.HasEnoughEnergy)
            {
                OnSkillCastFailed?.Invoke(new SkillCastFailedEvent(context.Owner, skill, SkillCastFailureReason.PrerequisiteNotMet));
                return SkillCastResult.Failure(skill, SkillCastFailureReason.PrerequisiteNotMet);
            }

            OnSkillCastStarted?.Invoke(new SkillCastStartedEvent(context.Owner, skill));
            skill.Execute();
            OnSkillCastSucceeded?.Invoke(new SkillCastSucceededEvent(context.Owner, skill));
            return SkillCastResult.Success(skill);
        }

        public bool HasReadySkill => readyQueue.Count > 0;

        public ActiveSkill PeekReadySkill()
        {
            return readyQueue.Count > 0 ? readyQueue.Peek() : null;
        }
    }
}