using Model.Tetri;
using UnityEngine;

namespace Units.Skills
{
    // 仿照 FlameRing 的结构，具体公式与行为先占位
    public class EnergyAbsorb : Skill, IPassiveSkill
    {
        public override CellTypeId CellTypeId => CellTypeId.EnergyAbsorb;

        public EnergyAbsorbConfig Config { get; }

        public EnergyAbsorb(EnergyAbsorbConfig config)
        {
            Config = config;
        }

        private struct EnergyAbsorbStats
        {
            public StatValue EnergyAbsorbPerSkillCast;
            public StatValue BuffDuration;
        }

        private EnergyAbsorbStats CalcStats()
        {
            int absorbCellCount = Owner != null && Owner.CellCounts.TryGetValue(AffinityType.Absorb, out var count) ? count : 0;

            return new EnergyAbsorbStats
            {
                EnergyAbsorbPerSkillCast = new StatValue("每次技能施放吸取能量", Config.BaseEnergyAbsorbPerSkillCast, absorbCellCount * Config.EnergyAbsorbPerAbsorbCell),
                BuffDuration = new StatValue("效果持续时间", Config.BuffDuration)
            };
        }

        public override string Description()
        {
            var s = CalcStats();
            return
                DescriptionStatic() + ":\n"
                + $"{s.EnergyAbsorbPerSkillCast}\n"
                ;
        }

        public static string DescriptionStatic() =>
            "被动：每当敌对单位施放技能时，汲取能量并分配到己方所有单位";


        public override string Name() => NameStatic();
        public static string NameStatic() => "能量汲取";

        public void ApplyPassive()
        {
            var stats = CalcStats();
            var buff = new Buffs.EnergyAbsorb(
                stats.EnergyAbsorbPerSkillCast.Final,
                stats.BuffDuration.Final,
                Owner,
                this
            );
            Owner.AddBuff(buff);
        }
    }
}