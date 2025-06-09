using System.Linq;
using UnityEngine;

namespace Units.Skills
{
    public class FlameRing : Skill
    {
        private bool hasTriggered;

        public float BaseDotDps = 2f;
        public float DotDpsPerFireCell = 1f;
        public float BaseDotDuration = 2f;
        public float DotDurationPerFireCell = 1f;
        public float BuffDuration = -1f;
        public float BaseRadius = 1f;
        public float RadiusPerFireCell = 0.2f;

        private int fireCellCount = 0;

        public FlameRing()
        {
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
            float dotDps = BaseDotDps + fireCellCount * DotDpsPerFireCell;
            float dotDuration = BaseDotDuration + fireCellCount * DotDurationPerFireCell;
            float radius = BaseRadius + fireCellCount * RadiusPerFireCell;

            var buff = new Buffs.FlameRing(
                dotDps,
                dotDuration,
                BuffDuration,
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