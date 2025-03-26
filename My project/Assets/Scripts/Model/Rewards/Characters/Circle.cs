
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
            return "decrease attak, increase health";
        }

        public override string GetName()
        {
            return "Character Circle";
        }
    }
}