using Model.Tetri;
using UnityEngine;

namespace Model.Rewards
{
    public class AttackFrequency : Tetri
    {
        public AttackFrequency()
        {
            InitializeCellTemplate<Model.Tetri.AttackFrequency>();
        }

        public override void FillCells()
        {
            SetRandomCell<Model.Tetri.AttackFrequency>();
        }
    }
}
