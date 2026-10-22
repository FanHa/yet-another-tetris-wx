using System.Collections.Generic;
using Model.Buffs;
using Units.Skills;
using UnityEngine;

namespace Units.Buffs
{
    public class EnergyAbsorb : Buff, IAnyUnitSkillCastTrigger
    {
        private readonly EnergyAbsorbBuffDefinition energyAbsorbDefinition;
        private readonly int resonantCellCount;
        private readonly float energyAbsorbPerSkillCast;
        public EnergyAbsorb(EnergyAbsorbBuffDefinition definition, int resonantCellCount, BuffSource source)
            : base(definition, definition.Duration, source.SourceUnit, source.SourceSkill)
        {
            energyAbsorbDefinition = definition;
            this.resonantCellCount = resonantCellCount;
            energyAbsorbPerSkillCast = definition.CalculateStats(resonantCellCount).EnergyAbsorbPerSkillCast.Final;
        }
        // 护盾对象
        public override string Description()
        {
            return energyAbsorbDefinition.DescribeEffect(resonantCellCount);
        }

        public override string Name()
        {
            return energyAbsorbDefinition.DisplayName;
        }

        public void OnAnyUnitSkillCast(IBuffContext context, Unit caster, Skill skill)
        {
            if (caster.faction != context.faction)
            {
                // 向己方所有活动单位分配能量
                IReadOnlyList<Unit> allies = context.UnitManager.GetActiveUnits(context.faction);
                foreach (var ally in allies)
                {
                    if (ally != context.SelfUnit)
                    {
                        ally.AddSkillEnergy(energyAbsorbPerSkillCast);
                    }
                }
            }
        }
    }
}