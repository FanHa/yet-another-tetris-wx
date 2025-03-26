using Model.Tetri;
using UnityEngine;

namespace Model.Rewards
{
    public class AttackInterval : Tetri
    {
        public override void FillCells()
        {
            foreach (var position in tetriInstance.GetOccupiedPositions())
            {
                tetriInstance.SetCell(position.x, position.y, new Model.Tetri.AttackInterval());
            }
        }

        public override string GetName() => "Attack Cooldown Reduction";
        public override string GetDescription() => "Decrease attack Cooldown";

    }
}
