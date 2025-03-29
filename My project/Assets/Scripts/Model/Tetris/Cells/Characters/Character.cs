using System;
using Units;
using UnityEngine;

namespace Model.Tetri
{
    [Serializable]
    public abstract class Character : Cell, ICharacterFeature
    {
        public abstract void ApplyFeatures(Unit unitComponent);

        public abstract string CharacterDescription();

    }
}