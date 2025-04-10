namespace Model.Rewards
{
    public class Spike : Tetri
    {
        private Model.Tetri.Skills.Spike cellTemplate = new Model.Tetri.Skills.Spike();

        public override void FillCells()
        {
            SetRandomCell<Model.Tetri.Skills.Spike>();
        }

        public override string Name() => cellTemplate.Name();
        public override string Description() => cellTemplate.Description();
    }
}
