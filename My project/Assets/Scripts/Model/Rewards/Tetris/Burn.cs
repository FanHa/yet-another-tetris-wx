namespace Model.Rewards
{
    public class Burn : Tetri
    {
        public Burn()
        {
            InitializeCellTemplate<Model.Tetri.Burn>();
        }

        public override void FillCells()
        {
            SetRandomCell<Model.Tetri.Burn>();
        }
    }
}