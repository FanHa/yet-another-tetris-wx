using System.Collections.Generic;
using UnityEngine;

namespace Units.Projectiles
{
    public abstract class PeriodicAreaEffectZoneBase : MonoBehaviour
    {
        [SerializeField] private Model.Tetri.AffinityDatabase affinityDatabase;
        [SerializeField] private ParticleSystem zoneParticle;
        [SerializeField] private ParticleSystem ringParticle;
        [SerializeField] private ParticleSystem ringOutlineParticle;

        private const uint RingParticleRandomSeed = 1347560529;
        private float radius;
        private float duration;
        [SerializeField, Min(0.01f)] private float applyInterval = 1f;
        private float elapsedTime;
        private float applyTimer;
        private bool isActive;

        protected Controller.UnitManager UnitManager { get; private set; }
        protected Unit SourceUnit { get; private set; }
        protected Skills.Skill SourceSkill { get; private set; }
        protected Model.Tetri.AffinityDatabase AffinityDatabase => affinityDatabase;
        protected float Radius => radius;

#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            if (affinityDatabase == null)
            {
                Debug.LogError($"{nameof(affinityDatabase)} is not assigned.", this);
            }

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

            if (!(applyInterval > 0f) || float.IsNaN(applyInterval) || float.IsInfinity(applyInterval))
            {
                Debug.LogError($"{nameof(applyInterval)} must be greater than 0 and finite.", this);
            }
        }
#endif

        protected void InitializeBase(
            Unit caster,
            float radius,
            float duration,
            Skills.Skill sourceSkill)
        {
            SourceUnit = caster;
            UnitManager = caster.UnitManager;
            this.radius = radius;
            this.duration = duration;
            SourceSkill = sourceSkill;
            elapsedTime = 0f;
            applyTimer = 0f;
            SyncParticleColor();
            SyncParticleRadius();
            ringParticle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            ringOutlineParticle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            ringParticle.useAutoRandomSeed = false;
            ringOutlineParticle.useAutoRandomSeed = false;
            ringParticle.randomSeed = RingParticleRandomSeed;
            ringOutlineParticle.randomSeed = RingParticleRandomSeed;
        }

        private void SyncParticleColor()
        {
            Model.Tetri.AffinityType affinity = SourceSkill.Definition.Affinity;
            if (affinity == Model.Tetri.AffinityType.None)
            {
                return;
            }

            Color color = AffinityDatabase.GetEntry(affinity).baseColor;
            SetParticleStartColor(zoneParticle, color);
            SetParticleStartColor(ringParticle, color);
            SetParticleStartColor(ringOutlineParticle, color);
        }

        private static void SetParticleStartColor(ParticleSystem particle, Color color)
        {
            var main = particle.main;
            main.startColor = color;
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
            bool isExpired = duration >= 0f && elapsedTime >= duration;
            if (!isExpired)
            {
                UpdateZonePosition();

                applyTimer += deltaTime;
                if (applyTimer >= applyInterval)
                {
                    applyTimer %= applyInterval;
                    ApplyEffect();
                }
            }

            if (isExpired)
            {
                Deactivate();
                Destroy(gameObject);
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

        protected virtual void UpdateZonePosition()
        {
        }

        public void Deactivate()
        {
            isActive = false;
        }
    }
}