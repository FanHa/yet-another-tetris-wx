using Model.Buffs;
using Units.Buffs;
using UnityEngine;

namespace Units.Projectiles
{
    public class CurseFieldZone : MonoBehaviour, Controller.IBattleEffect
    {
        [SerializeField] private ParticleSystem radiusSyncedParticle;

        private readonly string stackId = System.Guid.NewGuid().ToString("N");
        private Controller.UnitManager unitManager;
        private Unit sourceUnit;
        private Unit.Faction sourceFaction;
        private Skills.Skill sourceSkill;
        private BuffDefinition curseDefinition;
        private float radius;
        private float duration;
        private float applyInterval;
        private float debuffDuration;
        private float attackReductionPercent;
        private float energyRegenReductionPercent;
        private float elapsedTime;
        private float applyTimer;
        private bool isActive;

        public event System.Action<Controller.IBattleEffect> Deactivated;

        public void Initialize(
            Unit caster,
            float radius,
            float duration,
            float applyInterval,
            float debuffDuration,
            float attackReductionPercent,
            float energyRegenReductionPercent,
            Skills.Skill sourceSkill,
            BuffDefinition curseDefinition)
        {
            sourceUnit = caster;
            sourceFaction = caster.faction;
            unitManager = caster.UnitManager;
            this.radius = radius;
            this.duration = duration;
            this.applyInterval = applyInterval;
            this.debuffDuration = debuffDuration;
            this.attackReductionPercent = attackReductionPercent;
            this.energyRegenReductionPercent = energyRegenReductionPercent;
            this.sourceSkill = sourceSkill;
            this.curseDefinition = curseDefinition;
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
            ApplyCurse();
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
                return;
            }

            applyTimer += deltaTime;
            if (applyTimer >= applyInterval)
            {
                applyTimer %= applyInterval;
                ApplyCurse();
            }
        }

        private void ApplyCurse()
        {
            var enemies = unitManager.FindEnemiesInRangeAtPosition(sourceFaction, (Vector2)transform.position, radius);
            foreach (var enemy in enemies)
            {
                enemy.AddBuff(BuffFactory.Create(
                    curseDefinition,
                    new CurseBuffArgs(
                        debuffDuration,
                        attackReductionPercent,
                        energyRegenReductionPercent,
                        new BuffSource(sourceUnit, sourceSkill),
                        stackId)));
            }
        }

        public void Deactivate()
        {
            if (!isActive)
                return;

            isActive = false;
            Deactivated?.Invoke(this);
            Destroy(gameObject);
        }

        private void OnDestroy()
        {
            if (isActive)
            {
                isActive = false;
                Deactivated?.Invoke(this);
            }
        }
    }
}