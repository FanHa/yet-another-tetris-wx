namespace Model.Rewards
{
    public class Bomb : Tetri
    {
        public override void FillCells()
        {
            SetRandomCell<Model.Tetri.Skills.Bomb>();
        }

        public override string Name() => "爆破";
        public override string Description() => "attack add burn effect";
    }
}