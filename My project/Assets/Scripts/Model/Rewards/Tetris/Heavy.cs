using Model.Tetri;
using UnityEngine;

namespace Model.Rewards
{
    public class Heavy : Tetri
    {
        public override void FillCells()
        {
            foreach (var position in tetriInstance.GetOccupiedPositions())
            {
                tetriInstance.SetCell(position.x, position.y, new Model.Tetri.Heavy());
            }
        }

        public override string GetName() => "Heavy Boost";
        public override string GetDescription() => "Increases weight";

    }
}
