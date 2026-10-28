using System;
using Model.Resonance;
using Model.Skills;
using UnityEngine;

namespace Units.Skills
{
    /// <summary>
    /// 技能所属 Unit 的状态和操作门面。
    /// </summary>
    public sealed class SkillOwnerContext : ISkillOwnerContext
    {
        private readonly Unit owner;

        public SkillOwnerContext(Unit owner)
        {
            this.owner = owner ?? throw new ArgumentNullException(nameof(owner));
        }

        public Unit Owner => owner;

        public Attributes Attributes => owner.Attributes;
        public int GetResonanceCount(ResonanceType type) => owner.GetResonanceCount(type);
        public Transform Transform => owner.transform;
        public string Name => owner.name;
        public Transform ProjectileSpawnPoint => owner.projectileSpawnPoint;

        public void SetMoveBehaviorMode(Unit.MoveBehaviorMode mode)
        {
            owner.SetMoveBehaviorMode(mode);
        }

        public void Teleport(Vector3 position)
        {
            owner.Teleport(position);
        }

        public void AddBuff(Units.Buffs.Buff buff)
        {
            owner.AddBuff(buff);
        }

        public void TriggerAttackHit(Unit target, Units.Damages.Damage damage)
        {
            owner.TriggerAttackHit(target, damage);
        }

        public void TakeDamage(Units.Damages.Damage damage)
        {
            owner.TakeDamage(damage);
        }
    }
}