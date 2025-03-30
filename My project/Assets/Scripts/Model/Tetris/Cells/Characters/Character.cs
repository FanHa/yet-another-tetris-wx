using System;
using Units;
using UnityEngine;

namespace Model.Tetri
{
    [Serializable]
    public abstract class Character : Cell, ICharacterFeature
    {

        public abstract string CharacterDescription();

    }
}