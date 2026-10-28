using System;
using System.Collections.Generic;
using Model.Resonance;
using Model.Skills;
using UnityEngine;

namespace Units.Skills
{
    public interface ISkillOwnerContext
    {
        Unit Owner { get; }

        Attributes Attributes { get; }
        int GetResonanceCount(ResonanceType type);

        Transform Transform { get; }
        string Name { get; }
        Transform ProjectileSpawnPoint { get; }

        void SetMoveBehaviorMode(Unit.MoveBehaviorMode mode);
        void Teleport(Vector3 position);
        void AddBuff(Units.Buffs.Buff buff);
        void TriggerAttackHit(Unit target, Units.Damages.Damage damage);
        void TakeDamage(Units.Damages.Damage damage);
    }

    public interface ISkillTargeting
    {
        IReadOnlyList<Unit> GetActiveEnemies();
        IReadOnlyList<Unit> FindEnemiesInRange(float range);
        Unit FindRandomAlly();
        Unit FindRandomAllyIncludingSelf();
        Unit FindRandomAllyInRange(float range);
        Unit FindRandomAllyInRangeIncludingSelf(float range);
        Unit FindClosestEnemyInRange(float range);
        bool TryGetClosestEnemyInAttackRange(out Unit target);
    }

    public abstract class Skill
    {

        public abstract string Name();
        public abstract string DescribeEffect();

        public SkillDefinition Definition { get; internal set; }
        public ISkillOwnerContext OwnerContext { get; private set; }
        public ISkillTargeting Targeting { get; private set; }

        internal void Bind(ISkillOwnerContext ownerContext, ISkillTargeting targeting)
        {
            if (OwnerContext != null)
            {
                throw new InvalidOperationException("A skill can only be bound once.");
            }

            if (ownerContext == null)
            {
                throw new ArgumentNullException(nameof(ownerContext));
            }

            if (targeting == null)
            {
                throw new ArgumentNullException(nameof(targeting));
            }

            OwnerContext = ownerContext;
            Targeting = targeting;
        }

        public virtual void OnActivate() { }
        public virtual void OnDeactivate() { }
        public virtual void OnUnitDeath() {}
        public virtual void OnPassiveEffectsApplication() { }
        public virtual void OnRelease() {}
    }

}