using UnityEngine;

namespace Model.Rewards
{
    public class MultiAttack : Tetri
    {
        public MultiAttack()
        {
            InitializeCellTemplate<Model.Tetri.MultiAttack>();
        }

        public override void FillCells()
        {
            SetRandomCell<Model.Tetri.MultiAttack>();
        }
    }
}