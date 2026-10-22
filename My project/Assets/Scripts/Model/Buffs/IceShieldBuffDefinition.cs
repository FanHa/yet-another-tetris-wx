using UnityEngine;
using Units.Skills;

namespace Model.Buffs
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Buffs/Definitions/Ice Shield")]
    public sealed class IceShieldBuffDefinition : BuffDefinition
    {
        [SerializeField] private float duration = -1f;
        [SerializeField] private ChilledBuffDefinition chilledDefinition;

        public float Duration => duration;
        public ChilledBuffDefinition ChilledDefinition => chilledDefinition;

        public string DescribeEffect(int resonantCellCount) =>
            $"{new StatValue("护盾持续时间", duration)}\n被攻击时使攻击者获得Chilled：\n{chilledDefinition.DescribeEffect(resonantCellCount)}";

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            if (chilledDefinition == null)
            {
                Debug.LogError($"{GetType().Name} '{name}'.{nameof(chilledDefinition)} is not assigned.", this);
            }

            if (float.IsNaN(duration) || float.IsInfinity(duration) || (duration != -1f && duration <= 0f))
            {
                Debug.LogError($"{GetType().Name} '{name}'.{nameof(duration)} must be -1 (permanent) or finite and greater than zero.", this);
            }
        }
#endif
    }
}