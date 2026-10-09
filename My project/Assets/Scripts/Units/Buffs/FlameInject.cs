using Units.Skills;
using UnityEngine;

namespace Units.Buffs
{
    public readonly struct FlameInjectBuffArgs : IBuffCreationArgs
    {
        public Model.Buffs.BuffDefinition BurnDefinition { get; }
        public float DotDps { get; }
        public float DotDuration { get; }
        public float BuffDuration { get; }
        public BuffSource Source { get; }

        public FlameInjectBuffArgs(Model.Buffs.BuffDefinition burnDefinition, float dotDps, float dotDuration, float buffDuration, BuffSource source)
        {
            BurnDefinition = burnDefinition;
            DotDps = dotDps;
            DotDuration = dotDuration;
            BuffDuration = buffDuration;
            Source = source;
        }
    }

    /// <summary>
    /// FlameInject Buff：攻击时对目标附加火焰伤害并施加灼烧Dot
    /// </summary>
    public class FlameInject : Buff, IAttackHitTrigger
    {
        private float dotDps;
        private float dotDuration;
        private readonly Model.Buffs.BuffDefinition burnDefinition;

        public FlameInject(Model.Buffs.BuffDefinition definition, FlameInjectBuffArgs args)
            : base(definition, args.BuffDuration, args.Source.SourceUnit, args.Source.SourceSkill)
        {
            burnDefinition = args.BurnDefinition;
            dotDps = args.DotDps;
            dotDuration = args.DotDuration;
        }

        public override string Name() => "炎附";
        public override string Description() =>
            $"攻击时对目标附加{dotDps}/s灼烧({dotDuration}秒)";

        public void OnAttackHit(IBuffContext context, Unit attacker, Unit target, ref Damages.Damage damage)
        {
            var burn = BuffFactory.Create(
                burnDefinition,
                new BurnBuffArgs(
                    dotDps,
                    dotDuration,
                    new BuffSource(attacker, sourceSkill))
            );
            context.AddBuffTo(target, burn);
        }
    }
}