using Model.Tetri;
using UnityEngine;

namespace Model.Rewards
{
    public class Speed : Tetri
    {
        private Model.Tetri.Speed cellTemplate = new Model.Tetri.Speed();
        public override void FillCells()
        {
            foreach (var position in tetriInstance.GetOccupiedPositions())
            {
                tetriInstance.SetCell(position.x, position.y, new Model.Tetri.Speed());
            }
        }
        public override string GetName() => "兵贵神速";
        public override string GetDescription() => cellTemplate.Description();

    }
}
