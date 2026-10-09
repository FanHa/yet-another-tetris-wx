using Units.Skills;

namespace Units.Buffs
{
    public readonly struct FreezeBuffArgs : IBuffCreationArgs
    {
        public float Duration { get; }
        public BuffSource Source { get; }

        public FreezeBuffArgs(float duration, BuffSource source)
        {
            Duration = duration;
            Source = source;
        }
    }

    /// <summary>
    /// Freeze：完全冻结目标，无法移动、攻击、释放技能，能量回复为0
    /// </summary>
    public class Freeze : Buff
    {
        public Freeze(Model.Buffs.BuffDefinition definition, FreezeBuffArgs args)
            : base(definition, args.Duration, args.Source.SourceUnit, args.Source.SourceSkill)
        {
        }

        public override string Name() => "Freeze";
        public override string Description() => "完全冻结,无法行动,能量回复为0";

        public override void OnApply(IBuffContext context)
        {
            base.OnApply(context);
            // 攻速、移速、能量回复全部-100%
            context.Attributes.AttacksPerTenSeconds.AddPercentageModifier(this, -100);
            context.Attributes.MoveSpeed.AddPercentageModifier(this, -100);
            context.Attributes.ActionSpeed.AddPercentageModifier(this, -100);
            context.Attributes.EnergyPerSecond.AddPercentageModifier(this, -100);
        }

        public override void OnRemove()
        {
            context.Attributes.AttacksPerTenSeconds.RemovePercentageModifier(this);
            context.Attributes.MoveSpeed.RemovePercentageModifier(this);
            context.Attributes.ActionSpeed.RemovePercentageModifier(this);
            context.Attributes.EnergyPerSecond.RemovePercentageModifier(this);
            base.OnRemove();

        }
    }
}