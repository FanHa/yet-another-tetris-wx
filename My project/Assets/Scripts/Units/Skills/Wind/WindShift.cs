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

        private int GetResonantCellCount()
        {
            return Owner != null ? Owner.GetResonanceCount(Definition.Resonance) : 0;
        }
        public override string Description()
        {
            return
                DescriptionStatic() + ":\n" +
                ((WindShiftSkillConfig)Definition.Config).WindShiftBuffDefinition.DescribeEffect(GetResonantCellCount());
        }
 
        public static string DescriptionStatic() => "将单位的形态切换为风形态，提升攻击距离,获得攻击距离伤害加成";

        public override string Name()
        {
            return NameStatic();
        }
        public static string NameStatic() => "风形态";
    
        public override void OnApplyPassive()
        {
            Owner.AddBuff(new Buffs.WindShiftBuff(
                ((WindShiftSkillConfig)Definition.Config).WindShiftBuffDefinition,
                GetResonantCellCount(),
                new Buffs.BuffSource(Owner.SelfUnit, this)
            ));
        }
        
    }
}