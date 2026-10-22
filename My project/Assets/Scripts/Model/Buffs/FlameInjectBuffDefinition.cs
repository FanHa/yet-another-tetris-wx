using UnityEngine;

namespace Model.Buffs
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Buffs/Definitions/Flame Inject")]
    public sealed class FlameInjectBuffDefinition : BuffDefinition
    {
        [SerializeField] private BurnBuffDefinition burnDefinition;
        [SerializeField] private float duration = -1f;

        public BurnBuffDefinition BurnDefinition => burnDefinition;
        public float Duration => duration;

        public string DescribeEffect(int resonantCellCount)
        {
            var stats = burnDefinition.CalculateStats(resonantCellCount);
            return $"{stats.Dps}\n{stats.Duration}\n{new Units.Skills.StatValue("效果持续时间", duration)}";
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            if (burnDefinition == null)
            {
                Debug.LogError($"{GetType().Name} '{name}'.{nameof(burnDefinition)} is not assigned.", this);
            }

            if (float.IsNaN(duration) || float.IsInfinity(duration) || (duration != -1f && duration <= 0f))
            {
                Debug.LogError($"{GetType().Name} '{name}'.{nameof(duration)} must be -1 (permanent) or finite and greater than zero.", this);
            }
        }
#endif
    }
}