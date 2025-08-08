using System.Collections;
using UnityEngine;
using UnityEngine.Timeline;

namespace Units.Projectiles
{
    public class BlazingField : MonoBehaviour
    {
        [SerializeField] private ParticleSystem edgeParticle;
        [SerializeField] private ParticleSystem centerParticle;
        private Unit caster;
        private float radius;
        private float duration;
        private float dotDps;
        private float dotDuration;
        private const float tickInterval = 1f;
        private float timer = 0f;
        private float tickTimer = 0f;
        private bool initialized = false;
        private Skills.Skill sourceSkill;

        // [SerializeField] private ParticleSystem fireParticle;
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
                        var burn = new Units.Buffs.Burn(
                            dotDps,
                            dotDuration,
                            caster,
                            sourceSkill
                        );
                        enemy.AddBuff(burn);
                    }
                }
            }

            if (timer >= duration)
            {
                Destroy(gameObject);
            }
        }

        public void Init(Unit caster, float radius, float duration, float dotDps, float dotDuration, Skills.Skill sourceSkill)
        {
            this.caster = caster;
            this.radius = radius;
            this.duration = duration;
            this.dotDps = dotDps;
            this.dotDuration = dotDuration;
            this.sourceSkill = sourceSkill;

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

    }
}