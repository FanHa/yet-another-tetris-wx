using Model.Tetri;
using Model.Skills;
using Presentation;
using UnityEngine;

namespace Units.Skills
{
    public class LifePower : ActiveSkill
    {
        private readonly LifePowerSkillDefinition skillDefinition;

        public LifePower(LifePowerSkillDefinition definition)
        {
            skillDefinition = definition;
            RequiredEnergy = definition.RequiredEnergy;
        }

        protected override void ExecuteCore()
        {
            Unit targetAlly = UnitManager.FindRandomAlly(Owner) ?? Owner;

            var definition = skillDefinition.LifePowerBuffDefinition;
            var stats = definition.CalculateStats(
                Owner.Attributes.MaxHealth.finalValue,
                Owner.GetResonanceCount(Definition.Resonance)
            );
            targetAlly.AddBuff(
                new Units.Buffs.LifePowerBuff
                (
                    definition,
                    stats.AttackHitBonusDamage,
                    new Units.Buffs.BuffSource(Owner, this)
                )
            );
        }

        public override string DescribeEffect()
        {
            var definition = skillDefinition.LifePowerBuffDefinition;
            var stats = definition.CalculateStats(
                Owner.Attributes.MaxHealth.finalValue,
                Owner.GetResonanceCount(Definition.Resonance)
            );
            var previewBuff = new Units.Buffs.LifePowerBuff(
                definition,
                stats.AttackHitBonusDamage,
                new Units.Buffs.BuffSource(Owner, this)
            );
            return BuffDescriptionFormatter.Format(Definition.Description, previewBuff.DescribeAppliedEffect());
        }

        public override string Name() => Definition.DisplayName;
    }
}