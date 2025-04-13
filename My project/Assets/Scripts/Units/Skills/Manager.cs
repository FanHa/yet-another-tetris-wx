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

        public void Initialize(Unit owner)
        {
            this.owner = owner;
            foreach (var skill in skills)
            {
                skill.Init();
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

        public void CastSkills()
        {
            foreach (var skill in skills)
            {
                if (skill.IsReady())
                {
                    skill.Execute(owner);
                }
            }
        }
    }
}