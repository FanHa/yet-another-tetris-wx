 using System;
using Units;
using UnityEditor.Timeline.Actions;
using UnityEngine;

namespace Model.Tetri
{
    [Serializable]
    public class Speed : Attribute
    {
        [SerializeField]
        public float speed = 2;

        public override void ApplyAttributes(Unit unit)
        {
            unit.moveSpeed += speed;
        }

        public override string Description()
        {
            return "Speed: " + speed;
        }
    }
}