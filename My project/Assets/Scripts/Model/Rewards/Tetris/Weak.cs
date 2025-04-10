namespace Model.Rewards
{
    public class Weak : Tetri
    {
        public Weak()
        {
            InitializeCellTemplate<Model.Tetri.Skills.Weak>();
        }

        public override void FillCells()
        {
            SetRandomCell<Model.Tetri.Skills.Weak>();
        }
    }
}