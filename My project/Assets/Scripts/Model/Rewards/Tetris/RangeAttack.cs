namespace Model.Rewards
{
    public class RangeAttack : Tetri
    {
        private Model.Tetri.RangeAttack cellTemplate = new Model.Tetri.RangeAttack();

        public override void FillCells()
        {
            SetRandomCell<Model.Tetri.RangeAttack>();
        }

        public override string Name() => cellTemplate.Name();
        public override string Description() => cellTemplate.Description();
    }
}