using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Units.Skills
{
    public class Manager
    {
        private List<Skill> skills = new List<Skill>();
        private Unit owner;
        public int SkillsCount => skills.Count;
        
        public float CooldownRevisePercentage = 100f;
        private Skill readySkill;

        public void Initialize(Unit owner)
        {
            this.owner = owner;
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
                readySkill.Execute(owner); // 执行保存的技能
                readySkill = null; // 重置就绪技能
            }
        }

        public bool IsSkillReady()
        {
            if (readySkill != null)
            {
                return false;
            }
            readySkill = skills.FirstOrDefault(skill => skill.IsReady()); // 找到第一个就绪的技能
            return readySkill != null; // 如果找到就绪技能，返回 true

        }
    }
}