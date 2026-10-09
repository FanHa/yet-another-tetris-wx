using Units.Skills;
using Model.Buffs;

namespace Units.Buffs
{
    public readonly struct IceShieldBuffArgs : IBuffCreationArgs
    {
        public BuffDefinition ChilledDefinition { get; }
        public float BuffDuration { get; }
        public float ChilledDuration { get; }
        public int ChilledMoveSlowPercent { get; }
        public int ChilledAttackSlowPercent { get; }
        public int ChilledActionSlowPercent { get; }
        public int ChilledEnergyRegenSlowPercent { get; }
        public BuffSource Source { get; }

        public IceShieldBuffArgs(BuffDefinition chilledDefinition, float buffDuration, float chilledDuration, int chilledMoveSlowPercent, int chilledAttackSlowPercent, int chilledActionSlowPercent, int chilledEnergyRegenSlowPercent, BuffSource source)
        {
            ChilledDefinition = chilledDefinition;
            BuffDuration = buffDuration;
            ChilledDuration = chilledDuration;
            ChilledMoveSlowPercent = chilledMoveSlowPercent;
            ChilledAttackSlowPercent = chilledAttackSlowPercent;
            ChilledActionSlowPercent = chilledActionSlowPercent;
            ChilledEnergyRegenSlowPercent = chilledEnergyRegenSlowPercent;
            Source = source;
        }
    }

    /// <summary>
    /// 冰霜反击护盾：被攻击时对攻击者施加Chilled（多重减速）Debuff
    /// </summary>
    public class IceShield : Buff, ITakeHitTrigger
    {
        private float chilledDuration;         // Chilled Buff 持续时间（秒）
        private int chilledMoveSlowPercent;  // Chilled 移动速度减缓百分比
        private int chilledAttackSlowPercent;// Chilled 攻击速度减缓百分比
        private int chilledActionSlowPercent;// Chilled 动作速率减缓百分比
        private int chilledEnergyRegenSlowPercent; // Chilled 能量回复减缓百分比
        private readonly BuffDefinition chilledDefinition;

        public IceShield(BuffDefinition definition, IceShieldBuffArgs args)
            : base(definition, args.BuffDuration, args.Source.SourceUnit, args.Source.SourceSkill)
        {
            chilledDefinition = args.ChilledDefinition;
            chilledDuration = args.ChilledDuration;
            chilledMoveSlowPercent = args.ChilledMoveSlowPercent;
            chilledAttackSlowPercent = args.ChilledAttackSlowPercent;
            chilledActionSlowPercent = args.ChilledActionSlowPercent;
            chilledEnergyRegenSlowPercent = args.ChilledEnergyRegenSlowPercent;
        }

        public override string Name() => "冰霜护盾";
        public override string Description() =>
            $"被攻击时使攻击者获得Chilled:移速-{chilledMoveSlowPercent}%，攻速-{chilledAttackSlowPercent}%，动作速率-{chilledActionSlowPercent}%，能量回复-{chilledEnergyRegenSlowPercent}%，持续{chilledDuration}秒";

        public void OnTakeHit(IBuffContext context, Unit self, Unit attacker, ref Damages.Damage damage)
        {
            if (attacker == null)
                return;
            var chilled = BuffFactory.Create(
                chilledDefinition,
                new ChilledBuffArgs(
                    chilledDuration,
                    chilledMoveSlowPercent,
                    chilledAttackSlowPercent,
                    chilledActionSlowPercent,
                    chilledEnergyRegenSlowPercent,
                    new BuffSource(self, sourceSkill))
            );
            context.AddBuffTo(attacker, chilled);
        }

    }
}