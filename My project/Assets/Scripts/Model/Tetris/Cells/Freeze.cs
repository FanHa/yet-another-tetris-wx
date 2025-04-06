using System;
using Units;
namespace Model.Tetri
{
    [Serializable]
    public class Freeze : Cell, IBaseAttribute
    {
        public Units.Freeze freezeInstance = new Units.Freeze();
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
            return "技能:" + freezeInstance.Name();
        }
    }
}