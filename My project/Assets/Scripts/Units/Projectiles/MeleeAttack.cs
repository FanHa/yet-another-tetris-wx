using UnityEngine;

namespace Units.Projectiles
{
    public class MeleeAttack : MonoBehaviour
    {
        private Units.Unit caster;
        private Units.Unit target;
        private bool isActive = false;

        /// <summary>
        /// 初始化近战攻击
        /// </summary>
        public void Init(Units.Unit caster, Units.Unit target)
        {
            this.caster = caster;
            this.target = target;
        }

        public void Activate()
        {
            isActive = true;
        }

        private void Update()
        {
            if (!isActive)
                return;

            HandleHitTarget();
        }

        private void HandleHitTarget()
        {
            float damageValue = caster.Attributes.AttackPower.finalValue;
            var damage = new Damages.Damage(damageValue, Damages.DamageType.Hit);
            damage.SetSourceUnit(caster);
            damage.SetTargetUnit(target);
            damage.SetSourceLabel(Name());

            caster.TriggerAttackHit(target, damage);
            target.TakeHit(caster, ref damage);

            Destroy(gameObject);
        }
        
        private string Name()
        {
            return "近战攻击";
        }
    }
}