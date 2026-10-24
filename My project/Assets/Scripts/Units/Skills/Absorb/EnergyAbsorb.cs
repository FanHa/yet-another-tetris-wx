using Model.Resonance;
using Model.Skills;
using UnityEngine;

namespace Units.Skills
{
    // 仿照 FlameRing 的结构，具体公式与行为先占位
    public class EnergyAbsorb : Skill
    {
        public EnergyAbsorbLevelConfig Config { get; }

        public EnergyAbsorb(EnergyAbsorbLevelConfig config)
        {
            Config = config;
        }

        public override string Description()
        {
            return
                DescriptionStatic() + ":\n" +
                ((EnergyAbsorbSkillConfig)Definition.Config).EnergyAbsorbBuffDefinition.DescribeEffect(Context.GetResonanceCount(Definition.Resonance));
        }

        public static string DescriptionStatic() =>
            "被动：敌对单位施放技能时，为己方其他活动单位补充能量";


        public override string Name() => NameStatic();
        public static string NameStatic() => "能量汲取";

        public override void OnPassiveEffectsApplication()
        {
            var buff = new Buffs.EnergyAbsorb(
                ((EnergyAbsorbSkillConfig)Definition.Config).EnergyAbsorbBuffDefinition,
                Context.GetResonanceCount(Definition.Resonance),
                new Buffs.BuffSource(Context.Unit, this)
            );
            Context.AddBuff(buff);
        }
    }
}