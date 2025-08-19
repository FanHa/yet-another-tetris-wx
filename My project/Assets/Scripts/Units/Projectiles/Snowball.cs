using UnityEngine;

namespace Units.Projectiles
{
    public class Snowball : ProjectileToUnit
    {
        private float chilledDuration;
        private int moveSlowPercent;
        private int atkSlowPercent;
        private int energySlowPercent;
        private Units.Skills.Skill sourceSkill;

        public void Init(
            Units.Unit caster,
            Units.Unit target,
            float chilledDuration,
            int moveSlowPercent,
            int atkSlowPercent,
            int energySlowPercent,
            Units.Skills.Skill sourceSkill
        )
        {
            base.Init(caster, target);
            this.chilledDuration = chilledDuration;
            this.moveSlowPercent = moveSlowPercent;
            this.atkSlowPercent = atkSlowPercent;
            this.energySlowPercent = energySlowPercent;
            this.sourceSkill = sourceSkill;
        }

        protected override void HandleHitTarget()
        {
            // 施加冰霜减速Buff
            var chilledBuff = new Units.Buffs.Chilled(
                chilledDuration,
                moveSlowPercent,
                atkSlowPercent,
                energySlowPercent,
                caster,
                sourceSkill
            );
            target.AddBuff(chilledBuff);

            Destroy(gameObject);
        }
    }
}