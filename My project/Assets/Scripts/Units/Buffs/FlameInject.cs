using Units.Skills;
using UnityEngine;

namespace Units.Buffs
{
    /// <summary>
    /// FlameInject Buff：攻击时对目标附加火焰伤害并施加灼烧Dot
    /// </summary>
    public class FlameInject : Buff, IAttackHitTrigger
    {
        private float dotDps;
        private float dotDuration;

        public FlameInject(
            float dotDps,
            float dotDuration,
            float buffDuration,
            Unit sourceUnit,
            Skill sourceSkill
        ) : base(buffDuration, sourceUnit, sourceSkill)
        {
            this.dotDps = dotDps;
            this.dotDuration = dotDuration;
        }

        public override string Name() => "炎附";
        public override string Description() =>
            $"攻击时对目标附加{dotDps}/s灼烧({dotDuration}秒)";

        public void OnAttackHit(Unit attacker, Unit target, ref Damages.Damage damage)
        {
            var burn = new Burn(
                dps: dotDps,
                duration: dotDuration,
                sourceUnit: attacker,
                sourceSkill: sourceSkill
            );
            target.AddBuff(burn);
        }
    }
}