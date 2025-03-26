using Model.Tetri;
using UnityEngine;

namespace Model.Rewards
{
    public class Attack : Tetri
    {
        public override void FillCells()
        {
            foreach (var position in tetriInstance.GetOccupiedPositions())
            {
                tetriInstance.SetCell(position.x, position.y, new Model.Tetri.Attack());
            }
        }

        public override string GetName() => "Attack Boost";
        public override string GetDescription() => "Increases attack power";

    }
}
