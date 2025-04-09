using Model.Tetri;
using UnityEngine;

namespace Model.Rewards
{
    public class Attack : Tetri
    {
        private Model.Tetri.Attack cellTemplate = new Model.Tetri.Attack();

        public override void FillCells()
        {
            SetRandomCell<Model.Tetri.Attack>();
        }

        public override string Name() => "更锋利的武器";
        public override string Description() => $"{cellTemplate.Description()}";
    }
}
