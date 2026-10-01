using UnityEngine;
using Units.Projectiles;

namespace Model.Skills
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Skills/Configs/Common/Chain Lightning")]
    public class ChainLightningSkillConfig : SkillConfig
    {
        [Header("资源")]
        [SerializeField]
        private ChainLightning projectilePrefab;

        public ChainLightning ProjectilePrefab => projectilePrefab;

    #if UNITY_EDITOR
        private void OnValidate()
        {
            ValidateRequiredReference(projectilePrefab, nameof(projectilePrefab));
        }
    #endif
    }
}