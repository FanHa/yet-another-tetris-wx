using UnityEngine;
using Units.Projectiles;

namespace Model.Skills
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Skills/Configs/Life/Life Bomb")]
    public class LifeBombSkillConfig : SkillConfig<LifeBombLevelConfig>
    {
        [Header("资源")]
        [SerializeField]
        private LifeBomb projectilePrefab;

        public LifeBomb ProjectilePrefab => projectilePrefab;

        [SerializeField]
        private GameObject temporaryTargetPrefab;

        public GameObject TemporaryTargetPrefab => temporaryTargetPrefab;

#if UNITY_EDITOR
        protected override void ValidateSpecificConfig()
        {
            ValidateRequiredReference(projectilePrefab, nameof(projectilePrefab));
            ValidateRequiredReference(temporaryTargetPrefab, nameof(temporaryTargetPrefab));
            if (temporaryTargetPrefab == null || temporaryTargetPrefab.GetComponent<Collider2D>() != null)
            {
                return;
            }

            Debug.LogError($"{nameof(temporaryTargetPrefab)} must have a {nameof(Collider2D)} component.", this);
        }
#endif
    }

    [System.Serializable]
    public class LifeBombLevelConfig : SkillLevelConfig, IRequiresEnergy
    {
        [Header("通用")]
        public float RequiredEnergy;
        float IRequiresEnergy.RequiredEnergy => RequiredEnergy;

        [Header("属性")]
        public float HealthCostPercent;


    }
}
