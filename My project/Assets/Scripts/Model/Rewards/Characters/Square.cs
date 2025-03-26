
using UnityEngine;

namespace Model.Rewards
{
    public class Square : Character
    {
        public Square()
        {
            characterCellInstance = new Model.Tetri.Square();
        }

        public override string GetDescription()
        {
            return "Base character";
        }

        public override string GetName()
        {
            return "Character Square";
        }
    }
}