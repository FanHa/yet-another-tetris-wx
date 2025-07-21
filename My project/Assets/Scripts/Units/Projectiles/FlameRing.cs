using System.Linq;
using System.Threading;
using UnityEngine;

namespace Units.Projectiles
{
    public class FlameRing : MonoBehaviour
    {
        [SerializeField] private ParticleSystem ringParticle;
        private bool initialized = false;
        private Unit owner;
        private const float tickInterval = 1f;
        private float tickTimer = 0f;
        private float radius;
        private Skills.Skill sourceSkill;
        private float dotDps;
        private float dotDuration;

        public void Initialize(
            Unit owner,
            float radius,
            Skills.Skill sourceSkill,
            float dotDps,
            float dotDuration
        )
        {
            this.owner = owner;
            this.radius = radius;
            this.sourceSkill = sourceSkill;
            this.dotDps = dotDps;
            this.dotDuration = dotDuration;

            var shape = ringParticle.shape;
            shape.radius = radius;
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
            // todo Unit死亡时需要Destory这个FlameRing

            // 跟随目标
            transform.position = owner.transform.position;
            tickTimer += Time.deltaTime;
            if (tickTimer >= tickInterval)
            {
                tickTimer -= tickInterval;
                var enemies = owner.enemyUnits
                    .Where(u => u != null && Vector2.Distance(owner.transform.position, u.transform.position) <= radius)
                    .ToList();

                foreach (var enemy in enemies)
                {
                    var burn = new Units.Buffs.Burn(
                        dps: dotDps,
                        duration: dotDuration,
                        sourceUnit: owner,
                        sourceSkill: sourceSkill
                    );
                    enemy.AddBuff(burn);
                }
            }
        }
    }
}