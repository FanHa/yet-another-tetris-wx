namespace Model.Rewards
{
    public class Freeze : Tetri
    {
        private Model.Tetri.Freeze cellTemplate = new Model.Tetri.Freeze();

        public override void FillCells()
        {
            SetRandomCell<Model.Tetri.Freeze>();
        }

        public override string Name() => cellTemplate.Name();
        public override string Description() => cellTemplate.Description();
    }
}