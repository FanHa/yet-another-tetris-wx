using System;
using System.Reflection;
using Model.Tetri;

namespace Units.Skills
{
    public static class SkillFactory
    {
        public static Skill Create(SkillDefinition skillDefinition, int level)
        {
            Type skillType = typeof(Skill).Assembly.GetType($"Units.Skills.{skillDefinition.Id}");
            ConstructorInfo parameterlessConstructor = skillType.GetConstructor(Type.EmptyTypes);
            Skill skill;
            if (parameterlessConstructor != null)
            {
                skill = (Skill)parameterlessConstructor.Invoke(null);
            }
            else
            {
                MethodInfo getLevelConfig = skillDefinition.Config.GetType().GetMethod(nameof(SkillConfig<SkillLevelConfig>.GetLevelConfig));
                SkillLevelConfig levelConfig = (SkillLevelConfig)getLevelConfig.Invoke(skillDefinition.Config, new object[] { level });
                ConstructorInfo configuredConstructor = skillType.GetConstructor(new[] { levelConfig.GetType() });
                skill = (Skill)configuredConstructor.Invoke(new object[] { levelConfig });
            }

            skill.Definition = skillDefinition;
            return skill;
        }
    }
}
