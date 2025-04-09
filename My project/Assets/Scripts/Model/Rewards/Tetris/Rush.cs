namespace Model.Rewards
{
    public class Rush : Tetri
    {
        private Model.Tetri.Rush cellTemplate = new Model.Tetri.Rush();

        public override void FillCells()
        {
            SetRandomCell<Model.Tetri.Rush>();
        }

        public override string Name() => cellTemplate.Name();
        public override string Description() => cellTemplate.Description();
    }
}