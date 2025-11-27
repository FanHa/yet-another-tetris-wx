using System;
using Units;
using UnityEngine;

namespace Model.Tetri
{
    [Serializable]
    public class Health : Attribute
    {
        [SerializeField]
        private int CorePercentageModifier = 60;

        public override void Apply(Unit unit)
        {
            unit.Attributes.MaxHealth.AddPercentageModifier(this, CorePercentageModifier);
        }

        public override string Description()
        {
            return $"最大生命值 +{CorePercentageModifier}%";
        }

        public override string Name()
        {
            return "生命值";
        }
    }
}