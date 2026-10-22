using UnityEngine;
using UnityEngine.Serialization;
using Units.Projectiles;
using Model.Buffs;

namespace Model.Skills
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Skills/Configs/Ice/Ice Shield")]
    public class IceShieldSkillConfig : SkillConfig<IceShieldLevelConfig>
    {
        [Header("资源")]
        [SerializeField]
        private IceShield projectilePrefab;
        [SerializeField] private IceShieldBuffDefinition iceShieldBuffDefinition;

        public IceShield ProjectilePrefab => projectilePrefab;
        public IceShieldBuffDefinition IceShieldBuffDefinition => iceShieldBuffDefinition;

    #if UNITY_EDITOR
        protected override void ValidateSpecificConfig()
        {
            ValidateRequiredReference(projectilePrefab, nameof(projectilePrefab));
            ValidateRequiredReference(iceShieldBuffDefinition, nameof(iceShieldBuffDefinition));
        }
    #endif
    }

    [System.Serializable]
    public class IceShieldLevelConfig : SkillLevelConfig
    {
    }
}