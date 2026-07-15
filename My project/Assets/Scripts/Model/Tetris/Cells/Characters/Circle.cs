using System;
using UnityEngine;

namespace Model.Tetri
{
    [Serializable]
    public class Circle : Character
    {
        public override CharacterTypeId CharacterTypeId => CharacterTypeId.Circle;

        public override string Name()
        {
            return "小圆";
        }

        public override string Description()
        {
            return "血量极高，射程、攻击力和速度均较低。";
        }


    }
}