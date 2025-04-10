using UnityEngine;

namespace Model.Rewards
{
    public class FreezeShield : Tetri
    {
        public FreezeShield()
        {
            InitializeCellTemplate<Model.Tetri.Skills.FreezeShield>();
        }

        public override void FillCells()
        {
            SetRandomCell<Model.Tetri.Skills.FreezeShield>();
        }
    }
}