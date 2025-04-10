namespace Model.Rewards
{
    public class RangeAttack : Tetri
    {
        public RangeAttack()
        {
            InitializeCellTemplate<Model.Tetri.RangeAttack>();
        }

        public override void FillCells()
        {
            SetRandomCell<Model.Tetri.RangeAttack>();
        }
    }
}