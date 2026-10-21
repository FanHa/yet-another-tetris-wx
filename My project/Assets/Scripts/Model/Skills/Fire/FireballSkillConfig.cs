using UnityEngine;
using Units.Projectiles;

namespace Model.Skills
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Skills/Configs/Fire/Fireball")]
    public class FireballSkillConfig : SkillConfig<FireballLevelConfig>
    {
        [Header("资源")]
        [SerializeField]
        private Fireball projectilePrefab;

        public Fireball ProjectilePrefab => projectilePrefab;

    #if UNITY_EDITOR
        protected override void ValidateSpecificConfig()
        {
            ValidateRequiredReference(projectilePrefab, nameof(projectilePrefab));
        }
    #endif
    }

    [System.Serializable]
    public class FireballLevelConfig : SkillLevelConfig, IRequiresEnergy
    {
        [Header("通用")]
        public float RequiredEnergy;
        float IRequiresEnergy.RequiredEnergy => RequiredEnergy;
    }
}
