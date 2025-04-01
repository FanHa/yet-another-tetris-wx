using System;
using Units;
using UnityEditor.Timeline.Actions;
using UnityEngine;

namespace Model.Tetri
{
    [Serializable]
    public class Health : Cell, IBaseAttribute
    {
        [SerializeField]
        private int CorePercentageModifier = 60;

        public void ApplyAttributes(Unit unit)
        {
            unit.maxCore.AddPercentageModifier(this, CorePercentageModifier);
        }

        public override string Description()
        {
            return $"最大生命值 +{CorePercentageModifier}%";
        }
    }
}