using Units.Skills;
using UnityEngine;

namespace Units.Buffs
{
    public readonly struct ShadowAttackBuffArgs : IBuffCreationArgs
    {
        public Model.Buffs.BuffDefinition VulnerabilityDefinition { get; }
        public float VulnerabilityPercent { get; }
        public float DotDuration { get; }
        public BuffSource Source { get; }

        public ShadowAttackBuffArgs(Model.Buffs.BuffDefinition vulnerabilityDefinition, float vulnerabilityPercent, float dotDuration, BuffSource source)
        {
            VulnerabilityDefinition = vulnerabilityDefinition;
            VulnerabilityPercent = vulnerabilityPercent;
            DotDuration = dotDuration;
            Source = source;
        }
    }

    /// <summary>
    /// ShadowAttackBuff：攻击时对目标附加易伤Debuff
    /// </summary>
    public class ShadowAttackBuff : Buff, IAttackHitTrigger
    {
        private float vulnerabilityPercent;
        private float dotDuration;
        private readonly Model.Buffs.BuffDefinition vulnerabilityDefinition;

        public ShadowAttackBuff(Model.Buffs.BuffDefinition definition, ShadowAttackBuffArgs args)
            : base(definition, -1f, args.Source.SourceUnit, args.Source.SourceSkill) // -1f 表示永久Buff，可根据需要调整
        {
            vulnerabilityDefinition = args.VulnerabilityDefinition;
            vulnerabilityPercent = args.VulnerabilityPercent;
            dotDuration = args.DotDuration;
        }

        public override string Name() => "影袭";
        public override string Description() =>
            $"攻击时对目标施加{vulnerabilityPercent}%易伤Debuff，持续{dotDuration}秒";

        public void OnAttackHit(IBuffContext context, Unit attacker, Unit target, ref Damages.Damage damage)
        {
            var vulnerability = BuffFactory.Create(
                vulnerabilityDefinition,
                new VulnerabilityBuffArgs(dotDuration, vulnerabilityPercent, new BuffSource(attacker, sourceSkill))
            );
            context.AddBuffTo(target, vulnerability);
        }
    }
}