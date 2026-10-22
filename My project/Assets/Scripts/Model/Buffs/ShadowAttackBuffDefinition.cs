using UnityEngine;

namespace Model.Buffs
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Buffs/Definitions/Shadow Attack")]
    public sealed class ShadowAttackBuffDefinition : BuffDefinition
    {
        [SerializeField] private VulnerabilityBuffDefinition vulnerabilityDefinition;
        [SerializeField] private float duration = -1f;

        public VulnerabilityBuffDefinition VulnerabilityDefinition => vulnerabilityDefinition;
        public float Duration => duration;

        public string DescribeEffect(int resonantCellCount)
        {
            var stats = vulnerabilityDefinition.CalculateStats(resonantCellCount);
            return $"{stats.ExtraDamagePercent}\n{stats.Duration}\n{new Units.Skills.StatValue("效果持续时间", duration)}";
        }

        public string DescribeEffect(float extraDamagePercent, float duration) =>
            $"攻击时对目标施加{extraDamagePercent}%易伤Debuff，持续{duration}秒";

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            if (vulnerabilityDefinition == null)
            {
                Debug.LogError($"{GetType().Name} '{name}'.{nameof(vulnerabilityDefinition)} is not assigned.", this);
            }

            if (string.IsNullOrWhiteSpace(DisplayName))
            {
                Debug.LogError($"{GetType().Name} '{name}'.{nameof(DisplayName)} is not assigned.", this);
            }

            if (float.IsNaN(duration) || float.IsInfinity(duration) || (duration != -1f && duration <= 0f))
            {
                Debug.LogError($"{GetType().Name} '{name}'.{nameof(duration)} must be -1 (permanent) or finite and greater than zero.", this);
            }
        }
#endif
    }
}