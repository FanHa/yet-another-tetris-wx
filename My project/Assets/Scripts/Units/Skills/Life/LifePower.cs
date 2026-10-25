using Model.Tetri;
using Model.Skills;
using Presentation;
using UnityEngine;

namespace Units.Skills
{
    public class LifePower : ActiveSkill
    {
        public LifePowerLevelConfig Config { get; }
        private Unit cachedTarget;

        public LifePower(LifePowerLevelConfig config)
        {
            Config = config;
            RequiredEnergy = config.RequiredEnergy;
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

            var definition = ((LifePowerSkillConfig)Definition.Config).LifePowerBuffDefinition;
            float attackHitBonusDamage = definition.CalculateAttackHitBonusDamage(
                Context.Attributes.MaxHealth.finalValue,
                Context.GetResonanceCount(Definition.Resonance)
            );
            cachedTarget.AddBuff(
                new Units.Buffs.LifePowerBuff
                (
                    definition,
                    attackHitBonusDamage,
                    new Units.Buffs.BuffSource(Context.Unit, this)
                )
            );
            cachedTarget = null;
        }

        public override string DescribeEffect()
        {
            var definition = ((LifePowerSkillConfig)Definition.Config).LifePowerBuffDefinition;
            float attackHitBonusDamage = definition.CalculateAttackHitBonusDamage(
                Context.Attributes.MaxHealth.finalValue,
                Context.GetResonanceCount(Definition.Resonance)
            );
            var previewBuff = new Units.Buffs.LifePowerBuff(
                definition,
                attackHitBonusDamage,
                new Units.Buffs.BuffSource(Context.Unit, this)
            );
            return BuffDescriptionFormatter.Format(Definition.Description, previewBuff.DescribeAppliedEffect());
        }

        public override string Name()
        {
            return NameStatic();
        }
        public static string NameStatic() => "生命之力";
    }
}