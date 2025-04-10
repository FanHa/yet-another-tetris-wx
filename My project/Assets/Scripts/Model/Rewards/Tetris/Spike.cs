namespace Model.Rewards
{
    public class Spike : Tetri
    {
        public Spike()
        {
            InitializeCellTemplate<Model.Tetri.Spike>();
        }

        public override void FillCells()
        {
            SetRandomCell<Model.Tetri.Spike>();
        }
    }
}
