using UnityEngine;
using Model.Buffs;

namespace Model.Skills
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Skills/Configs/Life/Life Fortitude")]
    public class LifeFortitudeSkillConfig : SkillConfig<LifeFortitudeLevelConfig>
    {
        [SerializeField] private LifeFortitudeBuffDefinition buffDefinition;
        public LifeFortitudeBuffDefinition BuffDefinition => buffDefinition;

#if UNITY_EDITOR
        protected override void ValidateSpecificConfig()
        {
            ValidateRequiredReference(buffDefinition, nameof(buffDefinition));
            foreach (var (index, level) in GetValidLevelConfigs())
            {
                ValidateFinitePositive(level.MaxHealthIncrease, LevelField(index, nameof(level.MaxHealthIncrease)));
            }
        }

        private static string LevelField(int index, string fieldName) =>
            $"{nameof(LevelConfigs)}[{index}].{fieldName}";
#endif
    }

    [System.Serializable]
    public class LifeFortitudeLevelConfig : SkillLevelConfig
    {
        [Min(0.01f)] public float MaxHealthIncrease = 10f;
    }
}