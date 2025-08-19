using UnityEngine;

namespace Units.Projectiles
{
    public class ShadowArrow : ProjectileToUnit
    {
        private float vulnerabilityPercent;
        private float vulnerabilityDuration;
        private Units.Skills.Skill sourceSkill;


        /// <summary>
        /// 初始化暗影箭投射物
        /// </summary>
        public void Init(
            Units.Unit caster,
            Units.Unit target,
            float vulnerabilityPercent,
            float vulnerabilityDuration,
            Units.Skills.Skill sourceSkill
        )
        {
            base.Init(caster, target);
            this.vulnerabilityPercent = vulnerabilityPercent;
            this.vulnerabilityDuration = vulnerabilityDuration;
            this.sourceSkill = sourceSkill;
        }


        protected override void HandleHitTarget()
        {
            float damageValue = caster.Attributes.AttackPower.finalValue;
            var damage = new Damages.Damage(damageValue, Damages.DamageType.Hit);
            damage.SetSourceUnit(caster);
            damage.SetTargetUnit(target);
            damage.SetSourceLabel("暗影箭");
            target.TakeDamage(damage);

            var vulnerabilityBuff = new Units.Buffs.Vulnerability(
                vulnerabilityDuration,
                vulnerabilityPercent,
                caster,
                sourceSkill
            );
            target.AddBuff(vulnerabilityBuff);

            Destroy(gameObject);
        }

        private string Name()
        {
            return "暗影箭";
        }
    }
}