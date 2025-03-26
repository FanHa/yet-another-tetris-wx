

namespace Model.Rewards
{
    public class Triangle : Character
    {
        public Triangle()
        {
            characterCellInstance = new Model.Tetri.Triangle();
        }

        public override string GetDescription()
        {
            return "increase attak, decrease health";
        }

        public override string GetName()
        {
            return "Character Triangle";
        }
    }
}