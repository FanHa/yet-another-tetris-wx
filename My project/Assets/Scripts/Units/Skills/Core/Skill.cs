using System;
using System.Collections.Generic;
using System.Linq;
using Controller;
using Model.Resonance;
using Model.Skills;
using UnityEngine;

namespace Units.Skills
{
    public interface ISkillContext
    {
        Unit Unit { get; }

        Attributes Attributes { get; }
        int GetResonanceCount(ResonanceType type);

        Transform Transform { get; }
        string Name { get; }
        Transform ProjectileSpawnPoint { get; }

        IReadOnlyList<Unit> GetActiveEnemies();
        IReadOnlyList<Unit> FindEnemiesInRange(float range);
        Unit FindRandomAlly();
        Unit FindRandomAllyIncludingSelf();
        Unit FindRandomAllyInRange(float range);
        Unit FindRandomAllyInRangeIncludingSelf(float range);
        Unit FindClosestEnemyInRange(float range);
        bool TryGetClosestEnemyInAttackRange(out Unit target);

        void SetMoveBehaviorMode(Unit.MoveBehaviorMode mode);
        void Teleport(Vector3 position);
        Units.Movement.MovementResult MoveBy(Vector3 delta);
        void AddBuff(Units.Buffs.Buff buff);
    }

    public abstract class Skill
    {

        public abstract string Name();
        public abstract string DescribeEffect();

        public SkillDefinition Definition { get; internal set; }
        public ISkillContext Context { get; set; } // 技能的拥有者

        public virtual void OnActivate() { }
        public virtual void OnDeactivate() { }
        public virtual void OnUnitDeath() {}
        public virtual void OnPassiveEffectsApplication() { }
        public virtual void OnRelease() {}
    }

}