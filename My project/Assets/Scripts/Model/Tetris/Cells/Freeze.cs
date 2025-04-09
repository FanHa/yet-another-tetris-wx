using System;
using Units;
namespace Model.Tetri
{
    [Serializable]
    public class Freeze : Cell, IBaseAttribute
    {
        public Units.Buffs.Freeze freezeInstance = new Units.Buffs.Freeze();
        public void ApplyAttributes(Unit unit)
        {
            unit.attackEffects.Add(freezeInstance);
        }

        public override string Description()
        {
            return freezeInstance.Description();
        }

        public override string Name()
        {
            return freezeInstance.Name();
        }
    }
}