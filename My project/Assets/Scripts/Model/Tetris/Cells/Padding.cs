namespace Model.Tetri
{
    [System.Serializable]
    public class Padding : Cell
    {
        public override string Description()
        {
            return "没什么特别的用处.";
        }

        public override string Name()
        {
            return "填充物";
        }
    }
}