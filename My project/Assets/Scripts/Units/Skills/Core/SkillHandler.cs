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

        private readonly ISkillContext context;

        public event Action<SkillCastStartedEvent> OnSkillCastStarted;
        public event Action<SkillCastSucceededEvent> OnSkillCastSucceeded;
        public event Action<SkillCastFailedEvent> OnSkillCastFailed;

        private readonly List<Skill> skills = new();
        private readonly List<ActiveSkill> activeSkills = new();
        private readonly Queue<ActiveSkill> readyQueue = new();
        private readonly HashSet<ActiveSkill> queuedSet = new();

        public SkillHandler(ISkillContext context, float energyDecayPerSkill = 1f)
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

                float energyToDistribute = context.Attributes.EnergyPerSecond.finalValue * elapsedEnergyTime;
                DistributeEnergy(energyToDistribute);
            }
        }

        public void Activate()
        {
            isActive = true;
            tickTimer = 0f;
            readyQueue.Clear();
            queuedSet.Clear();
        }

        public void Deactivate()
        {
            isActive = false;
            tickTimer = 0f;
            readyQueue.Clear();
            queuedSet.Clear();
        }

        public void AddSkill(Skill newSkill)
        {
            if (skills.Any(skill => skill.GetType() == newSkill.GetType()))
                return;
            newSkill.Owner = context;
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


        public SkillCastResult TryCastNextSkill()
        {
            if (readyQueue.Count == 0)
            {
                OnSkillCastFailed?.Invoke(new SkillCastFailedEvent(context.SelfUnit, null, SkillCastFailureReason.NoPendingSkill));
                return SkillCastResult.Failure(null, SkillCastFailureReason.NoPendingSkill);
            }

            // 先出队，避免 Execute() 过程中队列被清空后再次 Dequeue 引发异常
            var skill = readyQueue.Dequeue();
            queuedSet.Remove(skill);

            if (!skill.HasEnoughEnergy)
            {
                OnSkillCastFailed?.Invoke(new SkillCastFailedEvent(context.SelfUnit, skill, SkillCastFailureReason.PrerequisiteNotMet));
                return SkillCastResult.Failure(skill, SkillCastFailureReason.PrerequisiteNotMet);
            }

            OnSkillCastStarted?.Invoke(new SkillCastStartedEvent(context.SelfUnit, skill));
            skill.Execute();
            OnSkillCastSucceeded?.Invoke(new SkillCastSucceededEvent(context.SelfUnit, skill));
            return SkillCastResult.Success(skill);
        }

        public bool HasReadySkill => readyQueue.Count > 0;

        public ActiveSkill PeekReadySkill()
        {
            return readyQueue.Count > 0 ? readyQueue.Peek() : null;
        }
    }
}