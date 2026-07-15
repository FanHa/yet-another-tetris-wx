using System;
using UnityEngine;

namespace Model.Tetri
{
    [Serializable]
    public class Hourglass : Character
    {
        public override CharacterTypeId CharacterTypeId => CharacterTypeId.Hourglass;

        public override string Name()
        {
            return "沙漏";
        }

        public override string Description()
        {
            return "生命值较低，但能量回复更高。";
        }
    }
}