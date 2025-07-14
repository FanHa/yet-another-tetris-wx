using System;
using System.Collections.Generic;
using System.Linq;

namespace Units.Skills
{
    public class SkillManager
    {
        public event Action<Skill> OnSkillReady;
        private List<Skill> skills = new();
        public IReadOnlyList<Skill> Skills => skills;
        
        public float EnergyDecayPerSkill { get; set; } = 0.8f;


        public void AddSkill(Skill newSkill)
        {
            if (skills.Any(skill => skill.GetType() == newSkill.GetType()))
                return;
            skills.Add(newSkill);
        }

        public void Tick(float energy)
        {
            int skillCount = skills.Count;
            if (skillCount == 0) return;
            float decay = MathF.Pow(EnergyDecayPerSkill, skillCount - 1);
            float gain = energy * decay;
            foreach (var skill in skills)
                skill.AddEnergy(gain);
            var readySkill = skills.FirstOrDefault(skill => skill.IsReady());
            if (readySkill != null)
            {
                OnSkillReady?.Invoke(readySkill);
            }
        }

    }
}