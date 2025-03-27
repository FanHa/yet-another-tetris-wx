using System;
using Units;
using UnityEngine;

namespace Model.Tetri
{
    [Serializable]
    public class MultiAttack : Attribute, ICharacterFeature
    {
        [SerializeField]
        public float attackTargetAddition = 1;
        [SerializeField]
        private float damageReductionPercentage = 50f; // 表示降低的百分比

        public override void ApplyAttributes(Unit unit)
        {
            unit.attackTargetNumber += attackTargetAddition;
        }


        public override string Description()
        {
            return $"Attack target Number: + {attackTargetAddition}, {damageReductionPercentage:F1}% damage reduction" ;
        }

        public void ApplyFeatures(Unit unitComponent)
        {
            // 降低伤害
            float damageMultiplier = 1 - (damageReductionPercentage / 100f);
            unitComponent.attackDamage *= damageMultiplier;
        }
    }
}