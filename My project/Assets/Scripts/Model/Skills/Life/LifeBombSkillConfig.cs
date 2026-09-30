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
    }

    [System.Serializable]
    public class LifeBombLevelConfig : SkillLevelConfig
    {
        [Header("通用")]
        public float RequiredEnergy;

        [Header("属性")]
        public float HealthCostPercent;


    }
}
