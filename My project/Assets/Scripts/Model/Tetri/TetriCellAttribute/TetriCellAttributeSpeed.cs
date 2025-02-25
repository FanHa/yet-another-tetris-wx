 using System;
using Units;
using UnityEditor.Timeline.Actions;
using UnityEngine;

namespace Model.Tetri
{
    [Serializable]
    public class TetriCellAttributeSpeed : TetriCellAttribute
    {
        [SerializeField]
        public float speed = 2;

        public override void ApplyAttributes(Unit unit)
        {
            unit.moveSpeed += speed;
        }
    }
}