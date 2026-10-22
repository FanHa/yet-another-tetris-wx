using UnityEngine;

namespace Model.Buffs
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Buffs/Definitions/Shadow Attack")]
    public sealed class ShadowAttackBuffDefinition : BuffDefinition
    {
        [SerializeField] private VulnerabilityBuffDefinition vulnerabilityDefinition;

        public VulnerabilityBuffDefinition VulnerabilityDefinition => vulnerabilityDefinition;

        public string DescribeEffect(int resonantCellCount)
        {
            var stats = vulnerabilityDefinition.CalculateStats(resonantCellCount);
            return $"{stats.ExtraDamagePercent}\n{stats.Duration}\n{BaseDuration.ToDisplayString("效果持续时间")}";
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

        }
#endif
    }
}