namespace Model.Rewards
{
    public class Weak : Tetri
    {
        private Model.Tetri.Weak cellTemplate = new Model.Tetri.Weak();

        public override void FillCells()
        {
            SetRandomCell<Model.Tetri.Weak>();
        }

        public override string Name() => cellTemplate.Name();
        public override string Description() => cellTemplate.Description();
    }
}