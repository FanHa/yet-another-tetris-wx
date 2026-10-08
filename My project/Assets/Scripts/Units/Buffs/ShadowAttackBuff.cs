using Units.Skills;
using UnityEngine;

namespace Units.Buffs
{
    /// <summary>
    /// ShadowAttackBuff：攻击时对目标附加易伤Debuff
    /// </summary>
    public class ShadowAttackBuff : Buff, IAttackHitTrigger
    {
        private float vulnerabilityPercent;
        private float dotDuration;
        private readonly Model.Buffs.BuffDefinition vulnerabilityDefinition;

        public ShadowAttackBuff(
            Model.Buffs.BuffDefinition definition,
            Model.Buffs.BuffDefinition vulnerabilityDefinition,
            float vulnerabilityPercent,
            float dotDuration,
            Unit sourceUnit,
            Skill sourceSkill
        ) : base(definition, -1f, sourceUnit, sourceSkill) // -1f 表示永久Buff，可根据需要调整
        {
            this.vulnerabilityDefinition = vulnerabilityDefinition;
            this.vulnerabilityPercent = vulnerabilityPercent;
            this.dotDuration = dotDuration;
        }

        public override string Name() => "影袭";
        public override string Description() =>
            $"攻击时对目标施加{vulnerabilityPercent}%易伤Debuff，持续{dotDuration}秒";

        public void OnAttackHit(IBuffContext context, Unit attacker, Unit target, ref Damages.Damage damage)
        {
            var vulnerability = new Vulnerability(
                vulnerabilityDefinition,
                buffDuration: dotDuration, // 可根据技能配置调整
                extraDamagePercent: vulnerabilityPercent,
                sourceUnit: attacker,
                sourceSkill: sourceSkill
            );
            context.AddBuffTo(target, vulnerability);
        }
    }
}