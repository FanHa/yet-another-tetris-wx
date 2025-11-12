using System.Linq;
using Model.Tetri;
using UnityEngine;

namespace Units.Skills
{
    // 仿照 FlameInject：被动技能。命中时若目标有 Chilled Buff -> 计算层数额外伤害并移除该 Buff
    public class IceBreaker : Skill, IPassiveSkill
    {
        public override CellTypeId CellTypeId => CellTypeId.IceBreaker;
        public IceBreakerConfig Config { get; }

        public IceBreaker(IceBreakerConfig config)
        {
            Config = config;
        }

        private struct IceBreakerStats
        {
            public StatValue BaseExtraDamage;
            public StatValue MultiplierByChilledLayer;
        }

        private IceBreakerStats CalcStats()
        {
            int iceCellCount = Owner != null && Owner.CellCounts.TryGetValue(AffinityType.Ice, out var c) ? c : 0;
            return new IceBreakerStats
            {
                BaseExtraDamage = new StatValue("基础额外伤害", Config.BaseExtraDamage + Config.ExtraDamagePerIceCell * iceCellCount),
                MultiplierByChilledLayer = new StatValue("chilled层数加成", Config.MultiplierByChilledLayer)
            };
        }

        public override string Description()
        {
            var s = CalcStats();
            return DescriptionStatic() + ":\n" +
                   $"{s.BaseExtraDamage}\n" +
                   $"{s.MultiplierByChilledLayer}\n";
        }

        public static string DescriptionStatic() => "攻击时若目标带有Chilled则引爆并清除该效果，按层数造成额外伤害";
        public override string Name() => NameStatic();
        public static string NameStatic() => "破冰";

        public void ApplyPassive()
        {
            var stats = CalcStats();
            Owner.AddBuff(new Buffs.IceBreaker(
                baseExtraDamage: stats.BaseExtraDamage.Final,
                percentSlowMultiplier: stats.MultiplierByChilledLayer.Final,
                buffDuration: -1f,
                sourceUnit: Owner,
                sourceSkill: this
            ));
        }


    }
}