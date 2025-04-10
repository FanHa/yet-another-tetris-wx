namespace Model.Rewards
{
    public class Bomb : Tetri
    {
        public Bomb()
        {
            InitializeCellTemplate<Model.Tetri.Skills.Bomb>();
        }

        public override void FillCells()
        {
            SetRandomCell<Model.Tetri.Skills.Bomb>();
        }
    }
}