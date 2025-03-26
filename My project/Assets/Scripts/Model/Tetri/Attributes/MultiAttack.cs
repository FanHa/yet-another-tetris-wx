using System;
using Units;
using UnityEngine;

namespace Model.Tetri
{
    [Serializable]
    public class MultiAttack : Attribute
    {
        [SerializeField]
        public float attackTargetAddition = 1;

        public override void ApplyAttributes(Unit unit)
        {
            unit.attackTargetNumber += attackTargetAddition;
        }

        public override string Description()
        {
            return "Attack target Number: +" + attackTargetAddition;
        }
    }
}