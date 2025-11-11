
using System.Collections.Generic;
using Model.Tetri;
using UnityEngine;

namespace Model
{
    public class CharacterPlacement
    {
        public CharacterInfluence CharacterInfluence { get; }

        public Vector3 RelativePositionFromCenter { get; }

        public CharacterPlacement(CharacterInfluence characterInfluence, Vector3 relativePositionFromCenter)
        {
            CharacterInfluence = characterInfluence;
            RelativePositionFromCenter = relativePositionFromCenter;
        }
    }
}