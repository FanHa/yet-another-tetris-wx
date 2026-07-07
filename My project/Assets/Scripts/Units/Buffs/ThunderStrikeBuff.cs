using Units.Skills;

namespace Units.Buffs
{
    /// <summary>
    /// ThunderStrike：雷击状态，期间单位进入眩晕
    /// </summary>
    public class ThunderStrikeBuff : Buff
    {
        public ThunderStrikeBuff(
            float duration,
            Unit sourceUnit,
            Skill sourceSkill
        ) : base(duration, sourceUnit, sourceSkill)
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
