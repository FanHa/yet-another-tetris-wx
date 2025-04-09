using Model.Tetri;
using UnityEngine;

namespace Model.Rewards
{
    public class AttackFrequency : Tetri
    {
        private Model.Tetri.AttackFrequency cellTemplate = new();

        public override void FillCells()
        {
            SetRandomCell<Model.Tetri.AttackFrequency>();
        }

        public override string Name() => cellTemplate.Name();
        public override string Description() => cellTemplate.Description();
    }
}
