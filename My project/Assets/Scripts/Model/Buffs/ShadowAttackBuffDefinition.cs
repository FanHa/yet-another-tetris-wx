using UnityEngine;

namespace Model.Buffs
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Buffs/Definitions/Shadow Attack")]
    public sealed class ShadowAttackBuffDefinition : BuffDefinition
    {
        [SerializeField] private VulnerabilityBuffDefinition vulnerabilityDefinition;

        public VulnerabilityBuffDefinition VulnerabilityDefinition => vulnerabilityDefinition;

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            if (vulnerabilityDefinition == null)
            {
                Debug.LogError($"{GetType().Name} '{name}'.{nameof(vulnerabilityDefinition)} is not assigned.", this);
            }

        }
#endif
    }
}