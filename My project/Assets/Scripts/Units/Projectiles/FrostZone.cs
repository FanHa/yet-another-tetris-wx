using UnityEngine;

namespace Units.Projectiles
{
    public class FrostZone : MonoBehaviour
    {
        [SerializeField] private ParticleSystem edgeParticle;
        [SerializeField] private ParticleSystem centerParticle;

        private Unit caster;
        private float radius;
        private float duration;
        private float damage;
        private float chilledDuration;
        private int moveSlowPercent;
        private int atkSlowPercent;
        private int energySlowPercent;
        private Units.Skills.Skill sourceSkill;

        private float timer = 0f;
        private float tickTimer = 0f;
        private const float tickInterval = 1f;
        private bool initialized = false;
        public void Initialize(
            Unit caster,
            float radius,
            float duration,
            float damage,
            float chilledDuration,
            int moveSlowPercent,
            int atkSlowPercent,
            int energySlowPercent,
            Units.Skills.Skill sourceSkill
        )
        {
            this.caster = caster;
            this.radius = radius;
            this.duration = duration;
            this.damage = damage;
            this.chilledDuration = chilledDuration;
            this.moveSlowPercent = moveSlowPercent;
            this.atkSlowPercent = atkSlowPercent;
            this.energySlowPercent = energySlowPercent;
            this.sourceSkill = sourceSkill;

            // 设置粒子特效半径
            var edgeShape = edgeParticle.shape;
            edgeShape.radius = radius;
        
            var centerShape = centerParticle.shape;
            centerShape.radius = radius;
            
            timer = 0f;
            tickTimer = 0f;
        }

        public void Activate()
        {
            initialized = true;
        }
        
        void Update()
        {
            if (!initialized)
                return;

            timer += Time.deltaTime;
            tickTimer += Time.deltaTime;

            if (tickTimer >= tickInterval)
            {
                tickTimer -= tickInterval;
                var enemies = caster.UnitManager != null
                    ? caster.UnitManager.FindEnemiesInRangeAtPosition(caster.faction, (Vector2)transform.position, radius)
                    : null;

                if (enemies != null)
                {
                    foreach (var enemy in enemies)
                    {
                        // 造成冰属性伤害
                        var iceDamage = new Damages.Damage(damage, Damages.DamageType.Skill);
                        iceDamage.SetSourceUnit(caster);
                        iceDamage.SetSourceLabel("霜域");
                        iceDamage.SetTargetUnit(enemy);
                        enemy.TakeDamage(iceDamage);

                        // 施加 Chilled Buff
                        var chilled = new Units.Buffs.Chilled(
                            chilledDuration,
                            moveSlowPercent,
                            atkSlowPercent,
                            energySlowPercent,
                            caster,
                            sourceSkill // 可传递技能引用
                        );
                        enemy.AddBuff(chilled);
                    }
                }
            }

            if (timer >= duration)
            {
                Destroy(gameObject);
            }
        }
    }
}