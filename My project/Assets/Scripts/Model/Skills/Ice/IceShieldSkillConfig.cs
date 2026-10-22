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
        [SerializeField] private BuffDefinition iceShieldBuffDefinition;
        [SerializeField] private ChilledBuffDefinition chilledBuffDefinition;

        public IceShield ProjectilePrefab => projectilePrefab;
        public BuffDefinition IceShieldBuffDefinition => iceShieldBuffDefinition;
        public ChilledBuffDefinition ChilledBuffDefinition => chilledBuffDefinition;

    #if UNITY_EDITOR
        protected override void ValidateSpecificConfig()
        {
            ValidateRequiredReference(projectilePrefab, nameof(projectilePrefab));
            ValidateRequiredReference(iceShieldBuffDefinition, nameof(iceShieldBuffDefinition));
            ValidateRequiredReference(chilledBuffDefinition, nameof(chilledBuffDefinition));
        }
    #endif
    }

    [System.Serializable]
    public class IceShieldLevelConfig : SkillLevelConfig
    {
        [Header("护盾与冰冻")]
        public float BuffDuration = -1f;
    }
}