using System.Linq;
using Model.Tetri;
using UnityEngine;

namespace Units.Skills
{
    public class FlameRing : Skill
    {
        public override CellTypeId CellTypeId => CellTypeId.FlameRing;

        private bool hasTriggered;
        private int fireCellCount = 0;
        public FlameRingConfig Config { get; }

        public FlameRing(FlameRingConfig config)
        {
            Config = config;
            hasTriggered = false;
        }

        public void SetFireCellCount(int fireCellCount)
        {
            this.fireCellCount = fireCellCount;
        }

        public override bool IsReady()
        {
            return !hasTriggered;
        }

        protected override void ExecuteCore(Unit caster)
        {
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
            TriggerEffect(new SkillEffectContext {
                Skill = this,  Radius = radius ,Target = caster
            });
            hasTriggered = true;
        }

        public override string Description()
        {
            return $"每次Tick对周围一圈敌人施加灼烧，所有效果随火系Cell数量提升。";
        }

        public override string Name()
        {
            return "火环";
        }
    }
}