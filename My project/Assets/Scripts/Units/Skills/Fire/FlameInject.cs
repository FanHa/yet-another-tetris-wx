using System.Linq;
using Model.Tetri;
using UnityEngine;

namespace Units.Skills
{
    public class FlameInject : Skill, IPassiveSkill
    {
        public override CellTypeId CellTypeId => CellTypeId.FlameInject;
        public FlameInjectConfig Config { get; }

        public FlameInject(FlameInjectConfig config)
        {
            Config = config;
        }       


        public override string Description()
        {
            return DescriptionStatic();
        }
        public static string DescriptionStatic() => "攻击时附加火焰伤害";

        public override string Name()
        {
            return NameStatic();
        }
        public static string NameStatic() => "炎附";

        public void ApplyPassive(Unit unit)
        {
            int fireCellCount = unit.CellCounts.TryGetValue(AffinityType.Fire, out var count) ? count : 0;
            float dotDps = Config.BaseDotDps + fireCellCount * Config.DotDpsPerFireCell;
            float dotDuration = Config.DotDuration;

            var buff = new Buffs.FlameInject(
                dotDps,
                dotDuration,
                Config.BuffDuration,
                unit,
                this
            );
            unit.AddBuff(buff);
        }

    }
}