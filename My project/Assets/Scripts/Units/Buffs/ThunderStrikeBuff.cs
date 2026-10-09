using Units.Skills;

namespace Units.Buffs
{
    public readonly struct ThunderStrikeBuffArgs : IBuffCreationArgs
    {
        public float Duration { get; }
        public BuffSource Source { get; }

        public ThunderStrikeBuffArgs(float duration, BuffSource source)
        {
            Duration = duration;
            Source = source;
        }
    }

    /// <summary>
    /// ThunderStrike：雷击状态，期间单位进入眩晕
    /// </summary>
    public class ThunderStrikeBuff : Buff
    {
        public ThunderStrikeBuff(Model.Buffs.BuffDefinition definition, ThunderStrikeBuffArgs args)
            : base(definition, args.Duration, args.Source.SourceUnit, args.Source.SourceSkill)
        {
        }

        public override string Name() => "ThunderStrike";
        public override string Description() => "雷击致晕，无法行动";

        public override void OnApply(IBuffContext context)
        {
            base.OnApply(context);
            context.EnterStun();
        }

        public override void OnRemove()
        {
            context.ExitStun();
            base.OnRemove();
        }
    }
}
