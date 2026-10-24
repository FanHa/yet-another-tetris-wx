using UnityEngine;

namespace Model.Buffs
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Buffs/Definitions/Ice Shield")]
    public sealed class IceShieldBuffDefinition : BuffDefinition
    {
        [SerializeField] private ChilledBuffDefinition chilledDefinition;

        public ChilledBuffDefinition ChilledDefinition => chilledDefinition;

        public string DescribeEffect(int resonantCellCount) =>
            $"{BaseDuration.ToDisplayString("护盾持续时间")}\n被攻击时使攻击者获得Chilled：\n{chilledDefinition.DescribeEffect(resonantCellCount)}";

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            if (chilledDefinition == null)
            {
                Debug.LogError($"{GetType().Name} '{name}'.{nameof(chilledDefinition)} is not assigned.", this);
            }

        }
#endif
    }
}