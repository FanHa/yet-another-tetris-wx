using Units.Buffs;

namespace Units.Buffs
{
    public class LifeEchoBuff : Buff, IAfterTakeDamageTrigger
    {
       private readonly float reflectPercent;

        /// <param name="reflectPercent">反弹伤害比例（0.2f 表示20%）</param>
        public LifeEchoBuff(Unit sourceUnit, Units.Skills.Skill sourceSkill, float reflectPercent)
            : base(-1f, sourceUnit, sourceSkill) // -1 表示永久Buff
        {
            this.reflectPercent = reflectPercent;
        }

        public override string Name() => "生命回响";

        public override string Description() => $"受到伤害后反弹{reflectPercent}%伤害给来源单位.";

        public void OnAfterTakeDamage(ref Damages.Damage damage)
        {
            if (damage.Type == Damages.DamageType.Reflect)
                return; // 避免反弹再次反弹
            // 反弹伤害给来源Unit
            var attacker = damage.SourceUnit;
            if (attacker != null && attacker != owner)
            {
                float reflectValue = damage.Value * (reflectPercent / 100f);
                var reflectDamage = new Damages.Damage(reflectValue, Damages.DamageType.Reflect)
                    .SetSourceUnit(owner)
                    .SetTargetUnit(attacker)
                    .SetSourceLabel("生命回响·反弹");
                attacker.TakeDamage(reflectDamage);
            }
        }
    }
}