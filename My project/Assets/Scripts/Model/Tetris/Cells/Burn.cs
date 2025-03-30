using System;
using Units;
namespace Model.Tetri
{
    [Serializable]
    public class Burn : Cell, IBaseAttribute
    {
        public Units.Burn burnInstance = new Units.Burn(); // 实例化一个Burn对象
        public void ApplyAttributes(Unit unit)
        {
            unit.attackEffects.Add(burnInstance);
        }

        public override string Description()
        {
            return "攻击附带灼烧效果,";
        }
    }
}