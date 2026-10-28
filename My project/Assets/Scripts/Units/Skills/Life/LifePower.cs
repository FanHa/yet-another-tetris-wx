using Model.Tetri;
using Model.Skills;
using Presentation;
using UnityEngine;

namespace Units.Skills
{
    public class LifePower : ActiveSkill
    {
        private readonly LifePowerSkillDefinition skillDefinition;
        private Unit cachedTarget;

        public LifePower(LifePowerSkillDefinition definition)
        {
            skillDefinition = definition;
            RequiredEnergy = definition.RequiredEnergy;
        }

        protected override bool PrepareCastCore()
        {
            cachedTarget = Targeting.FindRandomAlly() ?? OwnerContext.Owner;
            return true;
        }

        protected override void ExecuteCore()
        {
            if (cachedTarget == null)
            return;

            var definition = skillDefinition.LifePowerBuffDefinition;
            var stats = definition.CalculateStats(
                OwnerContext.Attributes.MaxHealth.finalValue,
                OwnerContext.GetResonanceCount(Definition.Resonance)
            );
            cachedTarget.AddBuff(
                new Units.Buffs.LifePowerBuff
                (
                    definition,
                    stats.AttackHitBonusDamage,
                    new Units.Buffs.BuffSource(OwnerContext.Owner, this)
                )
            );
            cachedTarget = null;
        }

        public override string DescribeEffect()
        {
            var definition = skillDefinition.LifePowerBuffDefinition;
            var stats = definition.CalculateStats(
                OwnerContext.Attributes.MaxHealth.finalValue,
                OwnerContext.GetResonanceCount(Definition.Resonance)
            );
            var previewBuff = new Units.Buffs.LifePowerBuff(
                definition,
                stats.AttackHitBonusDamage,
                new Units.Buffs.BuffSource(OwnerContext.Owner, this)
            );
            return BuffDescriptionFormatter.Format(Definition.Description, previewBuff.DescribeAppliedEffect());
        }

        public override string Name() => Definition.DisplayName;
    }
}