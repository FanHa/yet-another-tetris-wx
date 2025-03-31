using System;
using Units;
namespace Model.Tetri
{
    [Serializable]
    public class Freeze : Cell, IBaseAttribute
    {
        public Units.Freeze burnInstance = new Units.Freeze();
        public void ApplyAttributes(Unit unit)
        {
            unit.attackEffects.Add(burnInstance);
        }

        public override string Description()
        {
            return "攻击附带冰霜效果,";
        }
    }
}