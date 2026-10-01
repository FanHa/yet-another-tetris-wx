using UnityEngine;
using Units.Projectiles;

namespace Model.Skills
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Skills/Configs/Swift/Charge")]
    public class ChargeSkillConfig : SkillConfig<ChargeLevelConfig>
    {
        [Header("资源")]
        [SerializeField]
        private Charge projectilePrefab;

        public Charge ProjectilePrefab => projectilePrefab;

    #if UNITY_EDITOR
        private void OnValidate()
        {
            ValidateRequiredReference(projectilePrefab, nameof(projectilePrefab));
        }
    #endif
    }
    [System.Serializable]
    public class ChargeLevelConfig : SkillLevelConfig
    {
        public float RequiredEnergy;

        public float ChargeDamage;

        public float DamageBonusPerSpeed;

        public int AvoidancePriority = 0;

    }
}