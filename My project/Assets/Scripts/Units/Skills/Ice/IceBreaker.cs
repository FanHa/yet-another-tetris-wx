using System.Linq;
using Model.Resonance;
using Model.Skills;
using UnityEngine;

namespace Units.Skills
{
    // 仿照 FlameInject：被动技能。命中时若目标有 Chilled Buff -> 计算层数额外伤害并移除该 Buff
    public class IceBreaker : Skill
    {
        public IceBreakerLevelConfig Config { get; }

        public IceBreaker(IceBreakerLevelConfig config)
        {
            Config = config;
        }

        public override string Description()
        {
            return DescriptionStatic() + ":\n" +
                   ((IceBreakerSkillConfig)Definition.Config).IceBreakerBuffDefinition.DescribeEffect(Context.GetResonanceCount(Definition.Resonance));
        }

        public static string DescriptionStatic() => "攻击时若目标带有Chilled则引爆并清除该效果，按减速效果造成额外伤害";
        public override string Name() => NameStatic();
        public static string NameStatic() => "破冰";

        public override void OnPassiveEffectsApplication()
        {
            Context.AddBuff(new Buffs.IceBreaker(
                ((IceBreakerSkillConfig)Definition.Config).IceBreakerBuffDefinition,
                Context.GetResonanceCount(Definition.Resonance),
                new Buffs.BuffSource(Context.Unit, this)
            ));
        }


    }
}