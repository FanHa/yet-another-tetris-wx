using System;
using UnityEngine;

namespace Model.Tetri
{
    [Serializable]
    public class Triangle : Character
    {
        public override CharacterTypeId CharacterTypeId => CharacterTypeId.Triangle;

        public override string Description()
        {
            return "攻击力极高，生命值较低，移动速度较快。";
        }
    }
}