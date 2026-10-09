using Units.Buffs;

namespace Units.Buffs
{
    public readonly struct LifeEchoBuffArgs : IBuffCreationArgs
    {
        public BuffSource Source { get; }
        public float ReflectPercent { get; }

        public LifeEchoBuffArgs(float reflectPercent, BuffSource source)
        {
            Source = source;
            ReflectPercent = reflectPercent;
        }
    }

    public class LifeEchoBuff : Buff, IAfterTakeDamageTrigger
    {
       private readonly float reflectPercent;

        /// <param name="reflectPercent">反弹伤害比例（0.2f 表示20%）</param>
        public LifeEchoBuff(Model.Buffs.BuffDefinition definition, LifeEchoBuffArgs args)
            : base(definition, -1f, args.Source.SourceUnit, args.Source.SourceSkill) // -1 表示永久Buff
        {
            reflectPercent = args.ReflectPercent;
        }

        public override string Name() => "生命回响";

        public override string Description() => $"受到伤害后反弹{reflectPercent}%伤害给来源单位.";

        public void OnAfterTakeDamage(IBuffContext context, ref Damages.Damage damage)
        {
            if (damage.Type == Damages.DamageType.Hit || damage.Type == Damages.DamageType.Skill)
            {
                // 反弹伤害给来源Unit
                var attacker = damage.SourceUnit;
                if (attacker != null && attacker != context.SelfUnit)
                {
                    float reflectValue = damage.Value * (reflectPercent / 100f);
                    var reflectDamage = new Damages.Damage(reflectValue, Damages.DamageType.Reflect)
                        .SetSourceUnit(context.SelfUnit)
                        .SetTargetUnit(attacker)
                        .SetSourceLabel("生命回响·反弹");
                    context.DealDamageTo(attacker, reflectDamage);
                }
            }
        }
    }
}