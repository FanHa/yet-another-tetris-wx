
using UnityEngine;

namespace Model.Rewards
{
    public class Square : Character
    {
        public Square()
        {
            characterCellInstance = new Model.Tetri.Square();
        }

        public override string Description()
        {
            return characterCellInstance.Description();
        }

        public override string Name()
        {
            return characterCellInstance.Name();
        }
    }
}