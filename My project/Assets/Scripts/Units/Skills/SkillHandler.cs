using System;
using System.Collections.Generic;
using UnityEngine;

namespace Units.Skills
{
    public class SkillHandler : MonoBehaviour
    {
        public float energyPerTick;
        public float energyDecayPerSkill;
        private float tickTimer = 0f;
        private const float TICK_INTERVAL = 0.2f;
        private bool isActive = false;

        private SkillManager skillManager = new();
        private Unit owner;

        public event Action<Unit, Skill> OnSkillCast;
        public event Action<Skill> OnSkillReady;

        private readonly Queue<ActiveSkill> readyQueue = new Queue<ActiveSkill>();
        private readonly HashSet<ActiveSkill> queuedSet = new HashSet<ActiveSkill>();

        private Attributes attributes;

        public void Initialize(Attributes attributes)
        {
            this.attributes = attributes;
        }

        void Awake()
        {
            owner = GetComponent<Unit>();
            skillManager.EnergyDecayPerSkill = energyDecayPerSkill;
            skillManager.OnSkillReady += HandleSkillReady;
        }


        void Update()
        {
            if (!isActive) return;
            tickTimer += Time.deltaTime;
            if (tickTimer >= TICK_INTERVAL)
            {
                tickTimer -= TICK_INTERVAL;

                float energyPerTick = owner.Attributes.EnergyPerSecond.finalValue * TICK_INTERVAL;
                skillManager.Tick(energyPerTick);
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
            newSkill.Owner = owner; // 设置技能的拥有者
            skillManager.AddSkill(newSkill);
        }

        public IReadOnlyList<Skill> GetSkills()
        {
            return skillManager.Skills;
        }

        private void HandleSkillReady(ActiveSkill skill)
        {
            if (!queuedSet.Add(skill))
                return;

            readyQueue.Enqueue(skill);
            OnSkillReady?.Invoke(skill); // 用于 UI 高亮等
        }

        public void ExecutePendingSkill()
        {
            if (readyQueue.Count == 0) return;

            var skill = readyQueue.Peek();

            skill.Execute();
            OnSkillCast?.Invoke(owner, skill);


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