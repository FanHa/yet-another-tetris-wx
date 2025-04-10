using Model.Tetri;
using UnityEngine;

namespace Model.Rewards
{
    public class Speed : Tetri
    {
        public Speed()
        {
            InitializeCellTemplate<Model.Tetri.Speed>();
        }

        public override void FillCells()
        {
            SetRandomCell<Model.Tetri.Speed>();
        }
    }
}
