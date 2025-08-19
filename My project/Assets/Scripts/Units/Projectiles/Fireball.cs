namespace Units.Projectiles
{
    public class Fireball : ProjectileToUnit
    {
        private float burnDps;
        private float burnDuration;
        private Skills.Skill sourceSkill;

        public void Init(
            Units.Unit caster,
            Units.Unit target,
            float burnDps,
            float burnDuration,
            Units.Skills.Skill sourceSkill
        )
        {
            base.Init(caster, target);
            this.burnDps = burnDps;
            this.burnDuration = burnDuration;
            this.sourceSkill = sourceSkill;
        }

        protected override void HandleHitTarget()
        {
            // 命中后直接添加Burn Buff
            var burn = new Units.Buffs.Burn(
                dps: burnDps,
                duration: burnDuration,
                sourceUnit: caster,
                sourceSkill: sourceSkill
            );
            target.AddBuff(burn);

            Destroy(gameObject); // 销毁投射物
        }
    }
}