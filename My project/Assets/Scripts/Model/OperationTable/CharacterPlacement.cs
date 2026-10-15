
using System.Collections.Generic;
using Model.Tetri;
using UnityEngine;

namespace Model
{
    public class CharacterPlacement
    {
        public CharacterBuild CharacterBuild { get; }

        public Vector3 RelativePositionFromCenter { get; }

        public CharacterPlacement(CharacterBuild characterBuild, Vector3 relativePositionFromCenter)
        {
            CharacterBuild = characterBuild;
            RelativePositionFromCenter = relativePositionFromCenter;
        }
    }
}