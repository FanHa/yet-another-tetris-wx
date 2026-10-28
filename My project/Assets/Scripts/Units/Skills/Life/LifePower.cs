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
            cachedTarget = Context.FindRandomAlly() ?? Context.Unit;
            return true;
        }

        protected override void ExecuteCore()
        {
            if (cachedTarget == null)
            return;

            var definition = skillDefinition.LifePowerBuffDefinition;
            var stats = definition.CalculateStats(
                Context.Attributes.MaxHealth.finalValue,
                Context.GetResonanceCount(Definition.Resonance)
            );
            cachedTarget.AddBuff(
                new Units.Buffs.LifePowerBuff
                (
                    definition,
                    stats.AttackHitBonusDamage,
                    new Units.Buffs.BuffSource(Context.Unit, this)
                )
            );
            cachedTarget = null;
        }

        public override string DescribeEffect()
        {
            var definition = skillDefinition.LifePowerBuffDefinition;
            var stats = definition.CalculateStats(
                Context.Attributes.MaxHealth.finalValue,
                Context.GetResonanceCount(Definition.Resonance)
            );
            var previewBuff = new Units.Buffs.LifePowerBuff(
                definition,
                stats.AttackHitBonusDamage,
                new Units.Buffs.BuffSource(Context.Unit, this)
            );
            return BuffDescriptionFormatter.Format(Definition.Description, previewBuff.DescribeAppliedEffect());
        }

        public override string Name() => Definition.DisplayName;
    }
}