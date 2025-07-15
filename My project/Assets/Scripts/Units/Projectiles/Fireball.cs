namespace Units.Projectiles
{
    public class Fireball : Projectile
    {
        private float burnDps;
        private float burnDuration;
        private Skills.Skill sourceSkill;

        public void Init(Unit caster, Unit target, float burnDps, float burnDuration, Skills.Skill sourceSkill)
        {
            this.caster = caster;
            this.target = target.transform;
            this.burnDps = burnDps;
            this.burnDuration = burnDuration;
            this.sourceSkill = sourceSkill;
        }

        protected override void HandleHitTarget()
        {
            if (target == null)
            {
                Destroy(gameObject);
                return;
            }

            Unit unit = target.GetComponent<Units.Unit>();
            if (unit == null)
            {
                Destroy(gameObject);
                return;
            }

            // 命中后直接添加Burn Buff
            var burn = new Units.Buffs.Burn(
                dps: burnDps,
                duration: burnDuration,
                sourceUnit: caster,
                sourceSkill: sourceSkill
            );
            unit.AddBuff(burn);

            Destroy(gameObject); // 销毁投射物
        }
    }
}