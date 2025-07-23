using System.Linq;
using Model.Tetri;
using UnityEngine;

namespace Units.Skills
{
    public class FlameRing : Skill, IPassiveSkill
    {
        public override CellTypeId CellTypeId => CellTypeId.FlameRing;

        public FlameRingConfig Config { get; }

        public FlameRing(FlameRingConfig config)
        {
            Config = config;
        }

        public override string Description()
        {
            return DescriptionStatic();
        }
        public static string DescriptionStatic() => "对周围一圈敌人持续造成火焰伤害";

        public override string Name()
        {
            return NameStatic();
        }
        public static string NameStatic() => "火环";

        public void ApplyPassive(Unit caster)
        {
            int fireCellCount = caster.CellCounts.TryGetValue(AffinityType.Fire, out var count) ? count : 0;
            float dotDps = Config.BaseDotDps + fireCellCount * Config.DotDpsPerFireCell;
            float dotDuration = Config.BaseDotDuration + fireCellCount * Config.DotDurationPerFireCell;
            float radius = Config.BaseRadius + fireCellCount * Config.RadiusPerFireCell;

            var buff = new Buffs.FlameRing(
                dotDps,
                dotDuration,
                Config.BuffDuration,
                radius,
                caster,
                this
            );
            caster.AddBuff(buff);
        }
    }
}