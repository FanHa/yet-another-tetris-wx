
namespace Model.Rewards
{
    public class Circle : Character
    {
        public Circle()
        {
            characterCellInstance = new Model.Tetri.Circle();
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