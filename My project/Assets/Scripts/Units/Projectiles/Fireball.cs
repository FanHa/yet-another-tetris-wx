using Model.Buffs;
using Units.Buffs;
using Units.Skills;
using UnityEngine;

namespace Units.Projectiles
{
    public class Fireball : ProjectileToUnit
    {
        [SerializeField] private BurnBuffDefinition burnDefinition;
        private float burnDps;
        private float burnDuration;
        private BuffSource burnSource;

        public void Init(
            Units.Unit caster,
            Units.Unit target,
            Skill sourceSkill,
            int resonantCellCount
        )
        {
            base.Init(caster, target);
            var stats = burnDefinition.CalculateStats(resonantCellCount);
            burnDps = stats.Dps.Final;
            burnDuration = stats.Duration.Final;
            burnSource = new BuffSource(caster, sourceSkill);
        }

        public string DescribeEffect(int resonantCellCount)
        {
            var stats = burnDefinition.CalculateStats(resonantCellCount);
            return $"{stats.Dps}\n{stats.Duration}";
        }

        protected override void HandleHitTarget()
        {
            // 命中后直接添加Burn Buff
            var burn = new Burn(burnDefinition, burnDps, BuffDuration.ForSeconds(burnDuration), burnSource);
            target.AddBuff(burn);

            Destroy(gameObject); // 销毁投射物
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (burnDefinition == null)
            {
                Debug.LogError($"{GetType().Name} '{name}'.{nameof(burnDefinition)} is not assigned.", this);
            }
        }
#endif
    }
}