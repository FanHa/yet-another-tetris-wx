using UnityEngine;

namespace Model.Rewards
{
    public class FreezeShield : Tetri
    {
        private Model.Tetri.Skills.FreezeShield cellTemplate = new Model.Tetri.Skills.FreezeShield();

        public override void FillCells()
        {
            SetRandomCell<Model.Tetri.Skills.FreezeShield>();
        }

        public override string Name() => cellTemplate.Name();
        public override string Description() => cellTemplate.Description();
    }
}