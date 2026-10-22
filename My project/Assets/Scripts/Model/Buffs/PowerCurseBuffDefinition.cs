using UnityEngine;
using Units.Skills;

namespace Model.Buffs
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Buffs/Definitions/Power Curse")]
    public sealed class PowerCurseBuffDefinition : BuffDefinition
    {
        [SerializeField] private float baseAttackReductionPercent;
        [SerializeField] private float attackReductionPerResonantCell;
        [SerializeField] private float maxAttackReductionPercent;

        public (string Duration, StatValue AttackReductionPercent) CalculateStats(int resonantCellCount) =>
            (
                BaseDuration.ToDisplayString("诅咒持续"),
                new StatValue("攻击降低(%)", baseAttackReductionPercent, resonantCellCount * attackReductionPerResonantCell)
            );

        public string DescribeEffect(int resonantCellCount)
        {
            var stats = CalculateStats(resonantCellCount);
            return $"{Description}\n{stats.Duration}\n{stats.AttackReductionPercent}";
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            ValidateTimedBaseDuration();

            ValidatePercent(baseAttackReductionPercent, nameof(baseAttackReductionPercent));
            ValidatePercent(maxAttackReductionPercent, nameof(maxAttackReductionPercent));
            if (attackReductionPerResonantCell < 0f || float.IsNaN(attackReductionPerResonantCell) || float.IsInfinity(attackReductionPerResonantCell))
            {
                Debug.LogError($"{GetType().Name} '{name}'.{nameof(attackReductionPerResonantCell)} must be finite and non-negative.", this);
            }

            if (baseAttackReductionPercent > maxAttackReductionPercent)
            {
                Debug.LogError($"{GetType().Name} '{name}'.{nameof(baseAttackReductionPercent)} must not exceed {nameof(maxAttackReductionPercent)}.", this);
            }
        }

        private void ValidatePercent(float value, string fieldName)
        {
            if (float.IsNaN(value) || float.IsInfinity(value) || value < 0f || value > 100f)
            {
                Debug.LogError($"{GetType().Name} '{name}'.{fieldName} must be finite and between 0 and 100.", this);
            }
        }
#endif
    }
}