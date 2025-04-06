using Model.Tetri;
using UnityEngine;

namespace Model.Rewards
{
    public class AttackFrequency : Tetri
    {
        private Model.Tetri.AttackFrequency cellTemplate;

        public override void FillCells()
        {
            foreach (var position in tetriInstance.GetOccupiedPositions())
            {
                tetriInstance.SetCell(position.x, position.y, new Model.Tetri.AttackFrequency());
            }
        }

        public override string GetName() => cellTemplate.Name();
        public override string GetDescription() => cellTemplate.Description();

    }
}
