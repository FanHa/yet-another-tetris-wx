using System;
using Units;
namespace Model.Tetri.Skills
{
    [Serializable]
    public class PrecisionShoot : Cell, IBaseAttribute
    {
        public Units.Skills.PrecisionShoot skillInstance = new (); 
        public void ApplyAttributes(Unit unit)
        {
            unit.AddSkill(skillInstance);
        }

        public override string Description()
        {
            return skillInstance.Description();        
        }

        public override string Name()
        {
            return skillInstance.skillName;
        }
    }
}