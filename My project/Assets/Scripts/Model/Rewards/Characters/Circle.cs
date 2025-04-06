
namespace Model.Rewards
{
    public class Circle : Character
    {
        public Circle()
        {
            characterCellInstance = new Model.Tetri.Circle();
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