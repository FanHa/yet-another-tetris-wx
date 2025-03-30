using System;
using Units;
using UnityEngine;

namespace Model.Tetri
{
    [Serializable]
    // todo 拆分最终修改属性的Feature 与 character
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
            return $"Attack target Number: + {attackTargetAddition}, Distribute damage value to target number"; ;
        }

        public string CharacterDescription()
        {
            return Description();
        }
    }
}