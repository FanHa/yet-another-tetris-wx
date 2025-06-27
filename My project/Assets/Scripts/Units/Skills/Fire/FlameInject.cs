using System.Linq;
using UnityEngine;

namespace Units.Skills
{
    public class FlameInject : Skill
    {
        private bool hasTriggered;
        private int fireCellCount = 0;
        public FlameInjectConfig Config { get; }

        public FlameInject(FlameInjectConfig config)
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
            // 技能只在未触发过时才可用
            return !hasTriggered;
        }

        protected override void ExecuteCore(Unit caster)
        {
            float extraFireDamage = Config.BaseFireDamage + fireCellCount * Config.FireCellDamageBonus;
            float dotDps = Config.BaseDotDps + fireCellCount * Config.DotDpsPerFireCell;
            float dotDuration = Config.BaseDotDuration + fireCellCount * Config.DotDurationPerFireCell;

            var buff = new Buffs.FlameInject(
                extraFireDamage,
                dotDps,
                dotDuration,
                Config.BuffDuration,
                caster,
                this
            );
            caster.AddBuff(buff);
            hasTriggered = true; // 标记技能已触发

        }

        public override string Description()
        {
            return $"攻击时附加火焰伤害并施加灼烧，所有效果随火系Cell数量提升。";
        }

        public override string Name()
        {
            return "炎附";
        }
    }
}