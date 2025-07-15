using System.Collections;
using UnityEngine;
using UnityEngine.Timeline;

namespace Units.Projectiles
{
    public class BlazingField : MonoBehaviour
    {
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

        [SerializeField] private ParticleSystem fireParticle;
        void Update()
        {
            if (!initialized)
                return;
            timer += Time.deltaTime;
            tickTimer += Time.deltaTime;

            if (tickTimer >= tickInterval)
            {
                tickTimer -= tickInterval;
                // DOT判定
                Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius);
                foreach (var collider in colliders)
                {
                    Unit enemy = collider.GetComponent<Unit>();
                    if (enemy != null && enemy.faction != caster.faction)
                    {
                        var burn = new Units.Buffs.Burn(
                            dotDps,
                            dotDuration,
                            caster,
                            null // 可根据需要传递技能引用
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
            var main = fireParticle.main;
            main.startLifetime = radius;
            this.duration = duration;
            this.sourceSkill = sourceSkill;
            timer = 0f;
        }

        public void Activate()
        {
            initialized = true;
        }

    }
}