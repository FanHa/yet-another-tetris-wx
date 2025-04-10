namespace Model.Rewards
{
    public class Freeze : Tetri
    {
        public Freeze()
        {
            InitializeCellTemplate<Model.Tetri.Freeze>();
        }

        public override void FillCells()
        {
            SetRandomCell<Model.Tetri.Freeze>();
        }
    }
}