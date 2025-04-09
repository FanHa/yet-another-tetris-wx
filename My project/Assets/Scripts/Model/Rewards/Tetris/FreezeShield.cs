using UnityEngine;

namespace Model.Rewards
{
    public class FreezeShield : Tetri
    {
        private Model.Tetri.FreezeShield cellTemplate = new Model.Tetri.FreezeShield();

        public override void FillCells()
        {
            SetRandomCell<Model.Tetri.FreezeShield>();
        }

        public override string Name() => cellTemplate.Name();
        public override string Description() => cellTemplate.Description();
    }
}