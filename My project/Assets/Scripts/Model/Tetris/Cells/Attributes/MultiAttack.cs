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

        public override void Apply(Unit unit)
        {
            unit.Attributes.AttackTargetNumber += attackTargetAddition;
        }


        public override string Description()
        {
            return $"同时攻击目标 +{attackTargetAddition}, 将原有攻击力分摊到每个目标";
        }

        public override string Name()
        {
            return "技能: 多重攻击";
        }
    }
}