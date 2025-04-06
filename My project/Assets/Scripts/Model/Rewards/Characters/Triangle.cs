

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
            return characterCellInstance.Description();
        }

        public override string GetName()
        {
            return characterCellInstance.Name();
        }
    }
}