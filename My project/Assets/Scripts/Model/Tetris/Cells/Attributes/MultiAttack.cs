using System;
using Units;
using UnityEngine;

namespace Model.Tetri
{
    [Serializable]
    public class MultiAttack : Cell, IBaseAttribute
    {
        [SerializeField]
        public float attackTargetAddition = 1;

        public void ApplyAttributes(Unit unit)
        {
            unit.attackTargetNumber += attackTargetAddition;
        }


        public override string Description()
        {
            return $"Attack target Number: + {attackTargetAddition}, Distribute damage value to target number"; ;
        }

    }
}