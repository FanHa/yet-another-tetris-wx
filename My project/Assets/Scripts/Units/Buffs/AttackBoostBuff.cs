using Units.Skills;

namespace Units.Buffs
{
    public readonly struct AttackBoostBuffArgs : IBuffCreationArgs
    {
        public float Duration { get; }
        public float AtkSpeedPercent { get; }
        public BuffSource Source { get; }

        public AttackBoostBuffArgs(float duration, float atkSpeedPercent, BuffSource source)
        {
            Duration = duration;
            AtkSpeedPercent = atkSpeedPercent;
            Source = source;
        }
    }

    /// <summary>
    /// 疾风加速Buff：短时间提升攻击速度
    /// </summary>
    public class AttackBoostBuff : Buff
    {
        public float AtkSpeedPercent { get; }

        public AttackBoostBuff(Model.Buffs.BuffDefinition definition, AttackBoostBuffArgs args)
            : base(definition, args.Duration, args.Source.SourceUnit, args.Source.SourceSkill)
        {
            AtkSpeedPercent = args.AtkSpeedPercent;
        }

        public override string Name() => "疾风加速";
        public override string Description() => $"攻击速度提升{AtkSpeedPercent}%";

        public override void OnApply(IBuffContext context)
        {
            base.OnApply(context);
            context.Attributes.AttacksPerTenSeconds.AddPercentageModifier(this, AtkSpeedPercent);
        }

        public override void OnRemove()
        {
            context.Attributes.AttacksPerTenSeconds.RemovePercentageModifier(this);
            base.OnRemove();
        }
    }
}