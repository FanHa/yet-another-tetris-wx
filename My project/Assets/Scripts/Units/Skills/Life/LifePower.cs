using Model.Tetri;
using Model.Skills;
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

            cachedTarget.AddBuff(new Units.Buffs.LifePowerBuff(
                ((LifePowerSkillConfig)Definition.Config).LifePowerBuffDefinition,
                Context.Attributes.MaxHealth.finalValue,
                Context.GetResonanceCount(Definition.Resonance),
                new Units.Buffs.BuffSource(Context.Unit, this)
            ));
            cachedTarget = null;
        }

        public override string Description()
        {
            return
                DescriptionStatic() + "\n" +
                ((LifePowerSkillConfig)Definition.Config).LifePowerBuffDefinition.DescribeEffect(Context.GetResonanceCount(Definition.Resonance));
        }

        public static string DescriptionStatic() => "根据自身最大生命值，为一个友方单位附加攻击命中伤害";

        public override string Name()
        {
            return NameStatic();
        }
        public static string NameStatic() => "生命之力";
    }
}