using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Units.Skills
{
    public class Manager : MonoBehaviour
    {
        private List<Skill> skills = new List<Skill>();
        private Unit owner;
        public int SkillsCount => skills.Count;
        
        public float CooldownRevisePercentage = 100f;
        private Skill readySkill;

        public event Action<Unit, Skill> OnSkillCast;
        void Awake()
        {
            owner = GetComponent<Unit>();
            if (owner == null)
            {
                Debug.LogError("Unit component not found on the same GameObject as SkillsManager.");
            }
        }

        public void Init()
        {
            foreach (var skill in skills)
            {
                skill.Init(CooldownRevisePercentage);
            }
        }

        public void AddSkill(Skill newSkill)
        {
            // 检查是否已经存在相同类型的技能
            if (skills.Any(skill => skill.GetType() == newSkill.GetType()))
            {
                return;
            }

            skills.Add(newSkill);
        }

        public void CastSkill()
        {
            if (readySkill != null)
            {
                OnSkillCast?.Invoke(owner,readySkill);
                readySkill.Execute(owner); // 执行保存的技能
                readySkill = null; // 重置就绪技能
            }
        }

        public bool HasSkillToTrigger()
        {
            if (readySkill != null)
            {
                return false;
            }
            if (skills.Count <= 0) {
                return false;
            }
            readySkill = skills.FirstOrDefault(skill => skill.IsReady()); // 找到第一个就绪的技能
            return readySkill != null; // 如果找到就绪技能，返回 true

        }
    }
}