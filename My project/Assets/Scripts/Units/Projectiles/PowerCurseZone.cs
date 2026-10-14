using Model.Buffs;
using Units.Buffs;
using UnityEngine;

namespace Units.Projectiles
{
    public class PowerCurseZone : MonoBehaviour
    {
        [SerializeField] private ParticleSystem radiusSyncedParticle;

        private readonly string stackId = System.Guid.NewGuid().ToString("N");
        private Controller.UnitManager unitManager;
        private Unit sourceUnit;
        private Unit.Faction sourceFaction;
        private Skills.Skill sourceSkill;
        private BuffDefinition powerCurseBuffDefinition;
        private float radius;
        private float duration;
        private float applyInterval;
        private float debuffDuration;
        private float attackReductionPercent;
        private float elapsedTime;
        private float applyTimer;
        private bool isActive;

        public void Initialize(
            Unit caster,
            float radius,
            float duration,
            float applyInterval,
            float debuffDuration,
            float attackReductionPercent,
            Skills.Skill sourceSkill,
            BuffDefinition powerCurseBuffDefinition)
        {
            sourceUnit = caster;
            sourceFaction = caster.faction;
            unitManager = caster.UnitManager;
            this.radius = radius;
            this.duration = duration;
            this.applyInterval = applyInterval;
            this.debuffDuration = debuffDuration;
            this.attackReductionPercent = attackReductionPercent;
            this.sourceSkill = sourceSkill;
            this.powerCurseBuffDefinition = powerCurseBuffDefinition;
            elapsedTime = 0f;
            applyTimer = 0f;
            SyncParticleRadius();
        }

        private void SyncParticleRadius()
        {
            if (radiusSyncedParticle == null)
                return;

            var shape = radiusSyncedParticle.shape;
            if (shape.enabled)
            {
                shape.radius = radius;
            }
        }

        public void Activate()
        {
            if (isActive)
                return;

            isActive = true;
            ApplyPowerCurse();
        }

        private void Update()
        {
            Tick(Time.deltaTime);
        }

        internal void Tick(float deltaTime)
        {
            if (!isActive)
                return;

            elapsedTime += deltaTime;
            if (elapsedTime >= duration)
            {
                Deactivate();
                Destroy(gameObject);
                return;
            }

            applyTimer += deltaTime;
            if (applyTimer >= applyInterval)
            {
                applyTimer %= applyInterval;
                ApplyPowerCurse();
            }
        }

        private void ApplyPowerCurse()
        {
            var enemies = unitManager.FindEnemiesInRangeAtPosition(sourceFaction, (Vector2)transform.position, radius);
            foreach (var enemy in enemies)
            {
                enemy.AddBuff(BuffFactory.Create(
                    powerCurseBuffDefinition,
                    new PowerCurseBuffArgs(
                        debuffDuration,
                        attackReductionPercent,
                        new BuffSource(sourceUnit, sourceSkill),
                        stackId)));
            }
        }

        public void Deactivate()
        {
            isActive = false;
        }
    }
}