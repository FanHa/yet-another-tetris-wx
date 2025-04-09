using Model.Tetri;

namespace Model.Rewards
{
    public class Health : Tetri
    {
        private Model.Tetri.Health cellTemplate = new Model.Tetri.Health();

        public override void FillCells()
        {
            SetRandomCell<Model.Tetri.Health>();
        }

        public override string Name() => cellTemplate.Name();
        public override string Description() => cellTemplate.Description();
    }
}
