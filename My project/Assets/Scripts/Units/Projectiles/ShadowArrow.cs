using Model.Buffs;
using Units.Buffs;
using UnityEngine;

namespace Units.Projectiles
{
    public class ShadowArrow : ProjectileToUnit
    {
        [SerializeField] private VulnerabilityBuffDefinition vulnerabilityDefinition;
        private float vulnerabilityPercent;
        private float vulnerabilityDuration;
        private BuffSource vulnerabilitySource;


        /// <summary>
        /// 初始化暗影箭投射物
        /// </summary>
        public void Init(
            Units.Unit caster,
            Units.Unit target,
            Units.Skills.Skill sourceSkill,
            int resonantCellCount
        )
        {
            base.Init(caster, target);
            var stats = vulnerabilityDefinition.CalculateStats(resonantCellCount);
            vulnerabilityPercent = stats.ExtraDamagePercent.FinalValue;
            vulnerabilityDuration = stats.Duration.FinalValue;
            vulnerabilitySource = new BuffSource(caster, sourceSkill);
        }

        public string DescribeEffect(int resonantCellCount)
        {
            var stats = vulnerabilityDefinition.CalculateStats(resonantCellCount);
            return $"{stats.ExtraDamagePercent}\n{stats.Duration}";
        }

        protected override void HandleHitTarget()
        {
            float damageValue = caster.Attributes.AttackPower.finalValue;
            var damage = new Damages.Damage(damageValue, Damages.DamageType.Hit);
            damage.SetSourceUnit(caster);
            damage.SetTargetUnit(target);
            damage.SetSourceLabel("暗影箭");
            target.TakeDamage(damage);

            var vulnerabilityBuff = new Vulnerability(
                vulnerabilityDefinition,
                extraDamagePercent: vulnerabilityPercent,
                buffDuration: BuffDuration.ForSeconds(vulnerabilityDuration),
                source: vulnerabilitySource
            );
            target.AddBuff(vulnerabilityBuff);

            Destroy(gameObject);
        }

        private string Name()
        {
            return "暗影箭";
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (vulnerabilityDefinition == null)
            {
                Debug.LogError($"{GetType().Name} '{name}'.{nameof(vulnerabilityDefinition)} is not assigned.", this);
            }
        }
#endif
    }
}