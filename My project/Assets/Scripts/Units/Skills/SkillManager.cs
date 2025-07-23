using System;
using System.Collections.Generic;
using System.Linq;

namespace Units.Skills
{
    public class SkillManager
    {
        public event Action<ActiveSkill> OnSkillReady;
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
            var activeSkills = skills.OfType<ActiveSkill>().ToList();
            int activeSkillCount = activeSkills.Count;
            if (activeSkillCount == 0) return;
            float decay = MathF.Pow(EnergyDecayPerSkill, activeSkillCount - 1);
            float gain = energy * decay;
            foreach (ActiveSkill skill in activeSkills)
            {
                skill.AddEnergy(gain);
            }
            var readySkill = activeSkills.FirstOrDefault(skill => skill.IsReady());

            if (readySkill != null)
            {
                OnSkillReady?.Invoke(readySkill);
            }
        }

    }
}