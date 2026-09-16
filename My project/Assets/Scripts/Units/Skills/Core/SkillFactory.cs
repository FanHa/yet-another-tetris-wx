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
            if (parameterlessConstructor != null)
            {
                return (Skill)parameterlessConstructor.Invoke(null);
            }

            MethodInfo getLevelConfig = skillDefinition.Config.GetType().GetMethod(nameof(SkillConfig<SkillLevelConfig>.GetLevelConfig));
            SkillLevelConfig levelConfig = (SkillLevelConfig)getLevelConfig.Invoke(skillDefinition.Config, new object[] { level });
            ConstructorInfo configuredConstructor = skillType.GetConstructor(new[] { levelConfig.GetType() });
            return (Skill)configuredConstructor.Invoke(new object[] { levelConfig });
        }
    }
}
