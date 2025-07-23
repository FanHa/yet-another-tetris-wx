using System.Collections.Generic;
using Units;
using UnityEngine;

namespace Model.Tetri
{
    public class IceShield : Cell
    {
        public override CellTypeId CellTypeId => CellTypeId.IceShield;
        public override AffinityType Affinity => AffinityType.Ice;
        public override string Description() => Units.Skills.IceShield.DescriptionStatic();
        public override string Name() => Units.Skills.IceShield.NameStatic();

        public override void Apply(Unit unit)
        {
            var configGroup = SkillConfigGroup as Units.Skills.IceShieldConfigGroup;
            var config = configGroup?.LevelConfigs[Level - 1];
            var skillInstance = new Units.Skills.IceShield(config);
            unit.AddSkill(skillInstance);
        }
        
    }
}