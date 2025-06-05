using System;
using UnityEngine;

namespace Units.Skills
{
    public class SkillHandler : MonoBehaviour
    {
        public float energyPerTick = 5f;
        public float energyDecayPerSkill = 0.8f;
        private float tickTimer = 0f;
        private const float TICK_INTERVAL = 0.5f;
        private bool isActive = false;

        private SkillManager skillManager = new();
        private Unit owner;

        public event Action<Unit, Skill> OnSkillCast;
        public event Action<Skill> OnSkillReady;
        private Skill pendingSkill;

        void Awake()
        {
            owner = GetComponent<Unit>();
            skillManager.EnergyPerTick = energyPerTick;
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
                skillManager.Tick();
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
        }

        public void AddSkill(Skill newSkill) => skillManager.AddSkill(newSkill);

        private void HandleSkillReady(Skill skill)
        {
            if (pendingSkill != null)
            {
                return;
            }
            pendingSkill = skill;
            OnSkillReady?.Invoke(skill); // 让Unit监听这个事件
        }

        public void ExecutePendingSkill()
        {
            if (pendingSkill != null && pendingSkill.IsReady())
            {
                pendingSkill.Execute(owner);
                OnSkillCast?.Invoke(owner, pendingSkill);
                pendingSkill = null; // 清除已执行的技能
            }
        }

        internal Skill GetCurrentSkill()
        {
            return pendingSkill;
        }
    }
}