namespace Model.Tetri
{
    public class FirePadding : Cell
    {
        public FirePadding()
        {
            Affinity = AffinityType.Fire;
        }
        public override string Description()
        {
            return "火元素填充,增强火系单元格效果";
        }

        public override string Name()
        {
            return "火";
        }

    }
}