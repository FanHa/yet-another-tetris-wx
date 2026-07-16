using System;
using Units;
using UnityEngine;

namespace Model.Tetri
{
    [Serializable]
    public class Square : Character
    {
        public override CharacterTypeId CharacterTypeId => CharacterTypeId.Square;

        public override string Description()
        {
            return "平平无奇";
        }
    }
}