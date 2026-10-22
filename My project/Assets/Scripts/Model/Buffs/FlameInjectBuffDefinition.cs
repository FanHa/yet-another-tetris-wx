using UnityEngine;

namespace Model.Buffs
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Buffs/Definitions/Flame Inject")]
    public sealed class FlameInjectBuffDefinition : BuffDefinition
    {
        [SerializeField] private BurnBuffDefinition burnDefinition;

        public BurnBuffDefinition BurnDefinition => burnDefinition;

        public string DescribeEffect(int resonantCellCount)
        {
            var stats = burnDefinition.CalculateStats(resonantCellCount);
            return $"{stats.Dps}\n{stats.Duration}\n{BaseDuration.ToDisplayString("效果持续时间")}";
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            if (burnDefinition == null)
            {
                Debug.LogError($"{GetType().Name} '{name}'.{nameof(burnDefinition)} is not assigned.", this);
            }

        }
#endif
    }
}