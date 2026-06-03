using UnityEngine;

namespace Units.Projectiles
{
    public class ThunderStrikeProjectile : MonoBehaviour
    {
        [SerializeField] private float duration ;
        [SerializeField] private Vector3 headOffset;

        private Units.Unit caster;
        private Units.Unit target;
        private float damageValue;
        private float stunDuration;
        private Units.Skills.Skill sourceSkill;
        private bool isActive;
        private float timer;

        public void Init(
            Units.Unit caster,
            Units.Unit target,
            float damageValue,
            float stunDuration,
            Units.Skills.Skill sourceSkill
        )
        {
            this.caster = caster;
            this.target = target;
            this.damageValue = damageValue;
            this.stunDuration = stunDuration;
            this.sourceSkill = sourceSkill;

            if (this.target != null)
            {
                transform.position = this.target.transform.position + headOffset;
            }
        }

        public void Activate()
        {
            isActive = true;
            timer = 0f;
            if (target == null || !target.IsActive)
                return;

            var damage = new Damages.Damage(damageValue, Damages.DamageType.Skill)
                .SetSourceUnit(caster)
                .SetTargetUnit(target)
                .SetSourceLabel(sourceSkill != null ? sourceSkill.Name() : "雷击");
            target.TakeDamage(damage);

            var thunderStrikeBuff = new Units.Buffs.ThunderStrikeBuff(
                stunDuration,
                caster,
                sourceSkill
            );
            target.AddBuff(thunderStrikeBuff);
        }

        private void Update()
        {
            if (!isActive)
                return;

            if (target != null && target.IsActive)
            {
                transform.position = target.transform.position + headOffset;
            }

            timer += Time.deltaTime;
            if (timer >= duration)
            {
                Destroy(gameObject);
            }
        }
    }
}
