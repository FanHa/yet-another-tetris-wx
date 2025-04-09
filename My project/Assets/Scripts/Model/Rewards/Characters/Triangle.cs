

namespace Model.Rewards
{
    public class Triangle : Character
    {   
        public Triangle()
        {
            characterCellInstance = new Model.Tetri.Triangle();
        }

        public override string Description()
        {
            return characterCellInstance.Description();
        }

        public override string Name()
        {
            return characterCellInstance.Name();
        }
    }
}