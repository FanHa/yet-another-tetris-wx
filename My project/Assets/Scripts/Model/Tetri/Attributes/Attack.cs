 using System;
using Units;
using UnityEditor.Timeline.Actions;
using UnityEngine;

namespace Model.Tetri
{
    [Serializable]
    public class Attack : Attribute
    {
        [SerializeField]
        public float attack = 3;

        public override void ApplyAttributes(Unit unit)
        {
            unit.attackDamage += attack;
        }

        public override string Description()
        {
            return "Attack: " + attack;
        }

        
    }
}