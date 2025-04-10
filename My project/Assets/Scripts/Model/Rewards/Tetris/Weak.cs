namespace Model.Rewards
{
    public class Weak : Tetri
    {
        private Model.Tetri.Skills.Weak cellTemplate = new Model.Tetri.Skills.Weak();

        public override void FillCells()
        {
            SetRandomCell<Model.Tetri.Skills.Weak>();
        }

        public override string Name() => cellTemplate.Name();
        public override string Description() => cellTemplate.Description();
    }
}