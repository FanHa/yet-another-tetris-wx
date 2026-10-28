using System.Collections.Generic;
using Model;
using Model.Buffs;
using Presentation;
using Units.Skills;
using UnityEngine;

namespace Units.Buffs
{
    public class EnergyAbsorb : Buff, IAnyUnitSkillCastTrigger
    {
        private readonly EnergyAbsorbBuffDefinition energyAbsorbDefinition;
        private readonly AdditiveValueBreakdown energyAbsorbPerSkillCast;
        public EnergyAbsorb(EnergyAbsorbBuffDefinition definition, int resonantCellCount, BuffSource source)
            : base(definition, definition.BaseDuration, source.SourceUnit, source.SourceSkill)
        {
            energyAbsorbDefinition = definition;
            energyAbsorbPerSkillCast = definition.CalculateStats(resonantCellCount);
        }

        public override string DescribeAppliedEffect()
        {
            string durationDescription = Duration.ToDisplayString("效果持续时间");
            return energyAbsorbDefinition.Description + "\n" +
                   StatBreakdownFormatter.Format(StatLabel.EnergyPerEnemySkillCast, energyAbsorbPerSkillCast) + "\n" +
                   durationDescription;
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
                        ally.AddSkillEnergy(energyAbsorbPerSkillCast.FinalValue);
                    }
                }
            }
        }
    }
}