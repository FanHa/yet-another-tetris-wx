using Model.Tetri;
using UnityEngine;

namespace Model.Rewards
{
    public class Attack : Tetri
    {
        private Model.Tetri.Attack cellTemplate = new Model.Tetri.Attack();
        public override void FillCells()
        {
            foreach (var position in tetriInstance.GetOccupiedPositions())
            {
                tetriInstance.SetCell(position.x, position.y, new Model.Tetri.Attack());
            }
        }

        public override string GetName() => "更锋利的武器";
        public override string GetDescription() => $"{cellTemplate.Description()}";
    }
}
