namespace Model.Rewards
{
    public class PrecisionShoot : Tetri
    {
        public PrecisionShoot()
        {
            InitializeCellTemplate<Model.Tetri.Skills.PrecisionShoot>();
        }

        public override void FillCells()
        {
            SetRandomCell<Model.Tetri.Skills.PrecisionShoot>();
        }
    }
}