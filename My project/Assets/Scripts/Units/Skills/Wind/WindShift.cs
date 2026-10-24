using Model.Resonance;
using Model.Skills;

namespace Units.Skills
{
    public class WindShift : Skill
    {
        public WindShiftLevelConfig Config { get; }

        public WindShift(WindShiftLevelConfig config)
        {
            Config = config;
        }

        public override string Description()
        {
            return
                DescriptionStatic() + ":\n" +
                ((WindShiftSkillConfig)Definition.Config).WindShiftBuffDefinition.DescribeEffect(Context.GetResonanceCount(Definition.Resonance));
        }
 
        public static string DescriptionStatic() => "将单位的形态切换为风形态，提升攻击距离,获得攻击距离伤害加成";

        public override string Name()
        {
            return NameStatic();
        }
        public static string NameStatic() => "风形态";
    
        public override void OnPassiveEffectsApplication()
        {
            Context.AddBuff(new Buffs.WindShiftBuff(
                ((WindShiftSkillConfig)Definition.Config).WindShiftBuffDefinition,
                Context.GetResonanceCount(Definition.Resonance),
                new Buffs.BuffSource(Context.Unit, this)
            ));
        }
        
    }
}