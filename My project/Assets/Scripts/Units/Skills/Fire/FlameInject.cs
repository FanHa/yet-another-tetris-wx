using System.Linq;
using Model.Tetri;
using UnityEngine;

namespace Units.Skills
{
    public class FlameInject : Skill
    {
        public override CellTypeId CellTypeId => CellTypeId.FlameInject;
        private bool hasTriggered;
        public FlameInjectConfig Config { get; }

        public FlameInject(FlameInjectConfig config)
        {
            Config = config;
            hasTriggered = false;
        }

        public override bool IsReady()
        {
            // 技能只在未触发过时才可用
            return !hasTriggered;
        }

        protected override bool ExecuteCore(Unit caster)
        {
            int fireCellCount = caster.CellCounts.TryGetValue(AffinityType.Fire, out var count) ? count : 0;
            float dotDps = Config.BaseDotDps + fireCellCount * Config.DotDpsPerFireCell;
            float dotDuration = Config.DotDuration;

            var buff = new Buffs.FlameInject(
                dotDps,
                dotDuration,
                Config.BuffDuration,
                caster,
                this
            );
            caster.AddBuff(buff);
            hasTriggered = true; // 标记技能已触发
            return true;
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
    }
}