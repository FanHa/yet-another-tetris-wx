namespace Model.Rewards
{
    public class Spike : Tetri
    {
        private Model.Tetri.Spike cellTemplate = new Model.Tetri.Spike();

        public override void FillCells()
        {
            SetRandomCell<Model.Tetri.Spike>();
        }

        public override string Name() => cellTemplate.Name();
        public override string Description() => cellTemplate.Description();
    }
}
