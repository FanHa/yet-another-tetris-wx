namespace Model.Rewards
{
    public class Rush : Tetri
    {
        public Rush()
        {
            InitializeCellTemplate<Model.Tetri.Skills.Rush>();
        }

        public override void FillCells()
        {
            SetRandomCell<Model.Tetri.Skills.Rush>();
        }
    }
}