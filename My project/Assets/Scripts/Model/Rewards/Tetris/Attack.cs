using System.Runtime.InteropServices.WindowsRuntime;
using Model.Tetri;
using UnityEngine;

namespace Model.Rewards
{
    public class Attack : Tetri
    {
        public Attack()
        {
            InitializeCellTemplate<Model.Tetri.Attack>();
        }

        public override void FillCells()
        {
            SetRandomCell<Model.Tetri.Attack>();
        }
    }
}
