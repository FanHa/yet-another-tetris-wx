using System;
using UnityEngine;

namespace Model.Tetri
{
    [Serializable]
    public class Aim : Character
    {
        public override CharacterTypeId CharacterTypeId => CharacterTypeId.Aim;

        public override string Description()
        {
            return "天生远程单位,更多的攻击距离加成,更少的生命";
        }
    }
}