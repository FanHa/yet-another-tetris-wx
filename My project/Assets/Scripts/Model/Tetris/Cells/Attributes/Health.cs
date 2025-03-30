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
        private int HealthPercentageModifier = 60;

        public void ApplyAttributes(Unit unit)
        {
            unit.maxHPPercentageModifiers.Add(HealthPercentageModifier);
        }

        public override string Description()
        {
            return $"最大生命值 +{HealthPercentageModifier}%";
        }
    }
}