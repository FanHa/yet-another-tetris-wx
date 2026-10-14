using Model.Buffs;
using Units.Buffs;
using UnityEngine;

namespace Units.Projectiles
{
    public class PowerCurseZone : MonoBehaviour
    {
        [SerializeField] private ParticleSystem zoneParticle;
        [SerializeField] private ParticleSystem ringParticle;
        [SerializeField] private ParticleSystem ringOutlineParticle;

        private const uint RingParticleRandomSeed = 1347560529;
        private readonly string stackId = System.Guid.NewGuid().ToString("N");
        private Controller.UnitManager unitManager;
        private Unit sourceUnit;
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

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (zoneParticle == null)
            {
                Debug.LogError($"{nameof(zoneParticle)} is not assigned.", this);
            }

            if (ringParticle == null)
            {
                Debug.LogError($"{nameof(ringParticle)} is not assigned.", this);
            }

            if (ringOutlineParticle == null)
            {
                Debug.LogError($"{nameof(ringOutlineParticle)} is not assigned.", this);
            }
        }
#endif

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
            ringParticle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            ringOutlineParticle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            ringParticle.useAutoRandomSeed = false;
            ringOutlineParticle.useAutoRandomSeed = false;
            ringParticle.randomSeed = RingParticleRandomSeed;
            ringOutlineParticle.randomSeed = RingParticleRandomSeed;
        }

        private void SyncParticleRadius()
        {
            var shape = zoneParticle.shape;
            if (shape.enabled)
            {
                shape.radius = radius;
            }

            var ringShape = ringParticle.shape;
            if (ringShape.enabled)
            {
                ringShape.radius = radius;
            }

            var outlineShape = ringOutlineParticle.shape;
            if (outlineShape.enabled)
            {
                outlineShape.radius = radius;
            }
        }

        public void Activate()
        {
            if (isActive)
                return;

            isActive = true;
            ringParticle.Play(true);
            ringOutlineParticle.Play(true);
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
            var enemies = unitManager.FindEnemiesInRangeAtPosition(sourceUnit.faction, (Vector2)transform.position, radius);
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