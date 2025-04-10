namespace Model.Rewards
{
    public class Repel : Tetri
    {
        public Repel()
        {
            InitializeCellTemplate<Model.Tetri.Skills.Repel>();
        }

        public override void FillCells()
        {
            SetRandomCell<Model.Tetri.Skills.Repel>();
        }
    }
}