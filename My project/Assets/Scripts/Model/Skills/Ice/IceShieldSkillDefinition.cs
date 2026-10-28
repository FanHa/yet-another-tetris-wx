using Model.Buffs;
using Units.Projectiles;
using UnityEngine;

namespace Model.Skills
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Skills/Definitions/Ice/Ice Shield")]
    public sealed class IceShieldSkillDefinition : SkillDefinition
    {
        [Header("Resources")]
        [SerializeField] private IceShield projectilePrefab;
        [Header("Buff")]
        [SerializeField] private IceShieldBuffDefinition iceShieldBuffDefinition;

        public IceShield ProjectilePrefab => projectilePrefab;
        public IceShieldBuffDefinition IceShieldBuffDefinition => iceShieldBuffDefinition;

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            if (!ValidateDefinitionFields())
            {
                return;
            }

            ValidateRequiredReference(projectilePrefab, nameof(projectilePrefab));
            ValidateRequiredReference(iceShieldBuffDefinition, nameof(iceShieldBuffDefinition));
        }
#endif
    }
}