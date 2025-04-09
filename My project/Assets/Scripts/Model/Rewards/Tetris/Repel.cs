namespace Model.Rewards
{
    public class Repel : Tetri
    {
        private Model.Tetri.Repel cellTemplate = new();

        public override void FillCells()
        {
            SetRandomCell<Model.Tetri.Repel>();
        }

        public override string Name() => cellTemplate.Name();
        public override string Description() => cellTemplate.Description();
    }
}