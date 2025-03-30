using System;
using Units;
namespace Model.Tetri
{
    [Serializable]
    public class Burn : Cell, IBaseAttribute
    {
        public void ApplyAttributes(Unit unit)
        {
            throw new NotImplementedException();
        }

        public override string Description()
        {
            return "Burn";
        }
    }
}