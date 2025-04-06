
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
            return characterCellInstance.Description();
        }

        public override string GetName()
        {
            return characterCellInstance.Name();
        }
    }
}