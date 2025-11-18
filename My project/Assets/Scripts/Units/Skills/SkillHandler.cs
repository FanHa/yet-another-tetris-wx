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

        private Unit owner;

        public event Action<Unit, Skill> OnSkillCast;
        public event Action<Skill> OnSkillReady;

        private readonly List<Skill> skills = new();
        private readonly Queue<ActiveSkill> readyQueue = new();
        private readonly HashSet<ActiveSkill> queuedSet = new();

        void Awake()
        {
            owner = GetComponent<Unit>();
        }


        void Update()
        {
            if (!isActive) return;
            tickTimer += Time.deltaTime;
            if (tickTimer >= TICK_INTERVAL)
            {
                tickTimer -= TICK_INTERVAL;

                float energyPerTick = owner.Attributes.EnergyPerSecond.finalValue * TICK_INTERVAL;
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
            readyQueue.Clear();          // 清空就绪队列，避免残留
            queuedSet.Clear();
        }

        public void AddSkill(Skill newSkill)
        {
            if (skills.Any(skill => skill.GetType() == newSkill.GetType()))
                return;
            newSkill.Owner = owner; // 设置技能的拥有者
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

            var firstReady = activeSkills.FirstOrDefault(s => s.IsReady());
            if (firstReady != null && queuedSet.Add(firstReady))
            {
                readyQueue.Enqueue(firstReady);
                OnSkillReady?.Invoke(firstReady);
            }
        }


        public void ExecutePendingSkill()
        {
            if (readyQueue.Count == 0) return;

            var skill = readyQueue.Peek();

            bool result = skill.Execute();
            if (result)
            {
                OnSkillCast?.Invoke(owner, skill);
            }


            // 无论是否执行成功，都出队一次，允许后续重新判定再入队
            readyQueue.Dequeue();
            queuedSet.Remove(skill);
        }

        public bool HasReadySkill => readyQueue.Count > 0;
        public ActiveSkill PeekReadySkill()
        {
            return readyQueue.Count > 0 ? readyQueue.Peek() : null;
        }
    }
}