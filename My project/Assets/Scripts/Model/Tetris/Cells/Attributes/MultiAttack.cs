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
            return $"同时攻击目标 +{attackTargetAddition}, 将原有攻击力分摊到每个目标身上"; ;
        }

    }
}