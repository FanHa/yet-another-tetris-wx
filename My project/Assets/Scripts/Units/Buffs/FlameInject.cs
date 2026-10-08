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
        private readonly Model.Buffs.BuffDefinition burnDefinition;

        public FlameInject(
            Model.Buffs.BuffDefinition definition,
            Model.Buffs.BuffDefinition burnDefinition,
            float dotDps,
            float dotDuration,
            float buffDuration,
            Unit sourceUnit,
            Skill sourceSkill
        ) : base(definition, buffDuration, sourceUnit, sourceSkill)
        {
            this.burnDefinition = burnDefinition;
            this.dotDps = dotDps;
            this.dotDuration = dotDuration;
        }

        public override string Name() => "炎附";
        public override string Description() =>
            $"攻击时对目标附加{dotDps}/s灼烧({dotDuration}秒)";

        public void OnAttackHit(IBuffContext context, Unit attacker, Unit target, ref Damages.Damage damage)
        {
            var burn = new Burn(
                burnDefinition,
                dps: dotDps,
                duration: dotDuration,
                sourceUnit: attacker,
                sourceSkill: sourceSkill
            );
            context.AddBuffTo(target, burn);
        }
    }
}