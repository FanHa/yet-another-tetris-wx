using System.Collections.Generic;
using Model.Buffs;
using Units.Buffs;
using UnityEngine;

namespace Units.Projectiles
{
    public class FlameRing : MonoBehaviour
    {
        [SerializeField] private ParticleSystem ringParticle;
        private bool isActive;
        private Unit owner;
        private const float tickInterval = 1f;
        private float tickTimer = 0f;
        private float duration;
        private float elapsed;
        private float radius;
        private Skills.Skill sourceSkill;
        private BuffDefinition burnDefinition;
        private float dotDps;
        private float dotDuration;

        public void Initialize(
            Unit owner,
            float radius,
            Skills.Skill sourceSkill,
            BuffDefinition burnDefinition,
            float duration,
            float dotDps,
            float dotDuration
        )
        {
            this.owner = owner;
            this.radius = radius;
            this.sourceSkill = sourceSkill;
            this.burnDefinition = burnDefinition;
            this.duration = duration;
            this.dotDps = dotDps;
            this.dotDuration = dotDuration;

            var shape = ringParticle.shape;
            shape.radius = radius;
            tickTimer = 0f;
            elapsed = 0f;
        }

        public void Activate()
        {
            isActive = true;
            ringParticle.Play(true);
        }

        void Update()
        {
            if (!isActive)
                return;

            if (owner == null || !owner.IsActive)
            {
                Destroy(gameObject);
                return;
            }

            if (duration >= 0f)
            {
                elapsed += Time.deltaTime;
                if (elapsed >= duration)
                {
                    Destroy(gameObject);
                    return;
                }
            }

            // 跟随目标
            transform.position = owner.transform.position;
            tickTimer += Time.deltaTime;
            if (tickTimer >= tickInterval)
            {
                tickTimer -= tickInterval;

                Controller.UnitManager unitManager = owner.UnitManager;
                if (unitManager == null)
                    return;
                List<Units.Unit> enemies =  unitManager.FindEnemiesInRangeAtPosition(owner.faction, transform.position, radius);

                foreach (var enemy in enemies)
                {
                    var burn = BuffFactory.Create(
                        burnDefinition,
                        new BurnBuffArgs(dotDps, dotDuration, new BuffSource(owner, sourceSkill))
                    );
                    enemy.AddBuff(burn);
                }
            }
        }
    }
}