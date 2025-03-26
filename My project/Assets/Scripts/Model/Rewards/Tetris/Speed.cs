using Model.Tetri;
using UnityEngine;

namespace Model.Rewards
{
    public class Speed : Tetri
    {

        public override void FillCells()
        {
            foreach (var position in tetriInstance.GetOccupiedPositions())
            {
                tetriInstance.SetCell(position.x, position.y, new Model.Tetri.Speed());
            }
        }
        public override string GetName() => "Speed Boost";
        public override string GetDescription() => "Increases speed";

    }
}
