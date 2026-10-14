using System.Collections.Generic;
using UnityEngine;

namespace Units.Projectiles
{
    public abstract class PeriodicAreaEffectZoneBase : MonoBehaviour
    {
        [SerializeField] private ParticleSystem zoneParticle;
        [SerializeField] private ParticleSystem ringParticle;
        [SerializeField] private ParticleSystem ringOutlineParticle;

        private const uint RingParticleRandomSeed = 1347560529;
        private float radius;
        private float duration;
        private float applyInterval;
        private float elapsedTime;
        private float applyTimer;
        private bool isActive;

        protected Controller.UnitManager UnitManager { get; private set; }
        protected Unit SourceUnit { get; private set; }
        protected Skills.Skill SourceSkill { get; private set; }
        protected float Radius => radius;

#if UNITY_EDITOR
        protected virtual void OnValidate()
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

        protected void InitializeBase(
            Unit caster,
            float radius,
            float duration,
            float applyInterval,
            Skills.Skill sourceSkill)
        {
            SourceUnit = caster;
            UnitManager = caster.UnitManager;
            this.radius = radius;
            this.duration = duration;
            this.applyInterval = applyInterval;
            SourceSkill = sourceSkill;
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
            ApplyEffect();
        }

        protected virtual void Update()
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
                ApplyEffect();
            }
        }

        private void ApplyEffect()
        {
            var targets = FindTargetsInArea();
            foreach (var target in targets)
            {
                ApplyEffectTo(target);
            }
        }

        protected abstract IEnumerable<Unit> FindTargetsInArea();

        protected abstract void ApplyEffectTo(Unit target);

        public void Deactivate()
        {
            isActive = false;
        }
    }
}