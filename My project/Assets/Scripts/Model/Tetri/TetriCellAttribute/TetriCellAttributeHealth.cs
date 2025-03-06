using System;
using Units;
using UnityEditor.Timeline.Actions;
using UnityEngine;

namespace Model.Tetri
{
    [Serializable]
    public class TetriCellAttributeHealth : TetriCellAttribute
    {
        [SerializeField]
        public float health = 20;

        public override void ApplyAttributes(Unit unit)
        {
            unit.maxHP += health;
        }

        public override string Description()
        {
            return "Health: " + health;
        }
    }
}