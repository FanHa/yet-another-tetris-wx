using System.Linq;
using UnityEngine;

namespace Units.Skills
{
    public class FlameInject : Skill
    {
        private bool hasTriggered;

        public float BaseFireDamage = 1f;
        public float FireCellDamageBonus = 1f;
        public float BaseDotDps = 1f;
        public float DotDpsPerFireCell = 1f;
        public float BaseDotDuration = 2f;
        public float DotDurationPerFireCell = 1f;
        public float BuffDuration = -1f;

        private int fireCellCount = 0;

        public FlameInject()
        {
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
            float extraFireDamage = BaseFireDamage + fireCellCount * FireCellDamageBonus;
            float dotDps = BaseDotDps + fireCellCount * DotDpsPerFireCell;
            float dotDuration = BaseDotDuration + fireCellCount * DotDurationPerFireCell;

            var buff = new Buffs.FlameInject(
                extraFireDamage,
                dotDps,
                dotDuration,
                BuffDuration,
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