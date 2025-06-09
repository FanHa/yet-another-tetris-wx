using Units.Skills;
using UnityEngine;

namespace Units.Buffs
{
    /// <summary>
    /// FlameInject Buff：攻击时对目标附加火焰伤害并施加灼烧Dot
    /// </summary>
    public class FlameInject : Buff, IAttack
    {
        private float extraFireDamage;
        private float dotDps;
        private float dotDuration;

        public FlameInject(
            float extraFireDamage,
            float dotDps,
            float dotDuration,
            float buffDuration,
            Unit sourceUnit,
            Skill sourceSkill
        ) : base(buffDuration, sourceUnit, sourceSkill)
        {
            this.extraFireDamage = extraFireDamage;
            this.dotDps = dotDps;
            this.dotDuration = dotDuration;
        }

        public override string Name() => "炎附";
        public override string Description() =>
            $"攻击时对目标附加{extraFireDamage}点火焰伤害，并施加{dotDps}/s灼烧({dotDuration}秒)";

        public void OnAttack(Unit attacker, Unit target, ref Damages.Damage damage)
        {
            // 1. 直接附加火焰伤害
            var fireDamage = new Damages.Damage(extraFireDamage, Damages.DamageType.Fire);
            fireDamage.SetSourceUnit(attacker);
            fireDamage.SetTargetUnit(target);
            fireDamage.SetSourceLabel(Name());
            target.TakeDamage(fireDamage);

            // 2. 给目标添加灼烧Dot
            var dot = new Dot(
                DotType.Burn,
                skill: sourceSkill,
                caster: attacker,
                dps: dotDps,
                duration: dotDuration,
                label: "灼烧"
            );
            target.ApplyDot(dot);
        }
    }
}