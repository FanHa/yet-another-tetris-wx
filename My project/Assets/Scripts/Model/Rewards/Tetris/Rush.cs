namespace Model.Rewards
{
    public class Rush : Tetri
    {
        private Model.Tetri.Skills.Rush cellTemplate = new Model.Tetri.Skills.Rush();

        public override void FillCells()
        {
            SetRandomCell<Model.Tetri.Skills.Rush>();
        }

        public override string Name() => cellTemplate.Name();
        public override string Description() => cellTemplate.Description();
    }
}