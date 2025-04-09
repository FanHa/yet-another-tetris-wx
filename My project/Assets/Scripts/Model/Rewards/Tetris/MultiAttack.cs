using UnityEngine;

namespace Model.Rewards
{
    public class MultiAttack : Tetri
    {
        private Model.Tetri.MultiAttack cellTemplate = new Model.Tetri.MultiAttack();

        public override void FillCells()
        {
            SetRandomCell<Model.Tetri.MultiAttack>();
        }

        public override string Name() => cellTemplate.Name();
        public override string Description() => cellTemplate.Description();
    }
}