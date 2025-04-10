namespace Model.Rewards
{
    public class PrecisionShoot : Tetri
    {
        private Model.Tetri.Skills.PrecisionShoot cellTemplate = new();

        public override void FillCells()
        {
            SetRandomCell<Model.Tetri.Skills.PrecisionShoot>();
        }

        public override string Name() => cellTemplate.Name();
        public override string Description() => cellTemplate.Description();
    }
}