using UnityEngine;
using UnityEngine.Serialization;
using Units.Projectiles;
using Model.Buffs;

namespace Model.Skills
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Skills/Configs/Ice/Snowball")]
    public class SnowballSkillConfig : SkillConfig<SnowballLevelConfig>
    {
        [Header("资源")]
        [SerializeField]
        private Snowball projectilePrefab;

        public Snowball ProjectilePrefab => projectilePrefab;

    #if UNITY_EDITOR
        protected override void ValidateSpecificConfig()
        {
            ValidateRequiredReference(projectilePrefab, nameof(projectilePrefab));
        }
    #endif
    }

    [System.Serializable]
    public class SnowballLevelConfig : SkillLevelConfig, IRequiresEnergy
    {
        [Header("伤害")]
        public float BaseDamage = 10f;
        public float IceCellDamageBonus = 4f;

        [Header("消耗")]
        public float RequiredEnergy = 40f;
        float IRequiresEnergy.RequiredEnergy => RequiredEnergy;
    }
}
