using Model.Tetri;

namespace Model.Rewards
{
    public class Health : Tetri
    {
        public Health()
        {
            InitializeCellTemplate<Model.Tetri.Health>();
        }

        public override void FillCells()
        {
            SetRandomCell<Model.Tetri.Health>();
        }
    }
}
