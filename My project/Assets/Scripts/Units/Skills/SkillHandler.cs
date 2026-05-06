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
            readyQueue.Clear();
            queuedSet.Clear();
        }

        public void AddSkill(Skill newSkill)
        {
            if (skills.Any(skill => skill.GetType() == newSkill.GetType()))
                return;
            newSkill.Owner = owner;
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

            // 入队只检查能量是否充足，不调用 IsReady()。
            // 原因：部分技能在 IsReady() 中附加了"范围内是否有敌人"的目标查找副作用，
            // 若 0.2s tick 恰好无敌人在射程，IsReady() 返回 false，导致技能永远不进入队列，
            // CastSkillAction 永远无法启动。
            foreach (var s in activeSkills)
            {
                if (s.CurrentEnergy >= s.RequiredEnergy && queuedSet.Add(s))
                {
                    s.IsReady(); // 入队时做一次目标缓存（副作用），不依赖返回值
                    readyQueue.Enqueue(s);
                    OnSkillReady?.Invoke(s);
                }
            }
        }


        public void ExecutePendingSkill()
        {
            if (readyQueue.Count == 0) return;

            var skill = readyQueue.Peek();

            // 检查入队时锁定的目标是否仍然存活
            // 若目标已死亡，保留能量（不扣除）并出队，等待下次重新锁定新目标
            if (!skill.IsCachedTargetValid())
            {
                readyQueue.Dequeue();
                queuedSet.Remove(skill);
                return;
            }

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