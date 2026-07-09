using System;
using System.Collections.Generic;
using System.Linq;
using Controller;
using Model.Tetri;
using UnityEngine;

namespace Units.Skills
{
    public interface ISkillContext
    {
        Unit SelfUnit { get; }

        Attributes Attributes { get; }
        Dictionary<AffinityType, int> CellCounts { get; }
        Model.ProjectileConfig ProjectileConfig { get; }

        Transform transform { get; }
        string name { get; }
        Transform projectileSpawnPoint { get; }

        Coroutine StartCoroutine(System.Collections.IEnumerator routine);

        List<Unit> FindEnemiesInRange(float range);
        Unit FindRandomAlly(float range, bool includeSelf = false);
        Unit FindClosestEnemyInRange(float range);
        Unit FindLowestMaxHealthEnemy();
        Unit FindFurthestEnemy();

        bool TryGetClosestEnemyInAttackRange(out Unit target);
        bool TryGetClosestEnemy(out Unit target);
        bool TryGetClosestAlly(out Unit target);

        void SetMoveBehaviorMode(Unit.MoveBehaviorMode mode);
        void Teleport(Vector3 position);
        Units.Movement.MovementResult MoveBy(Vector3 delta);
        void AddBuff(Units.Buffs.Buff buff);
    }

    public abstract class Skill
    {

        public virtual CellTypeId CellTypeId => CellTypeId.None; // 默认值，子类可以覆盖

        public abstract string Name();
        public abstract string Description();

        public ISkillContext Owner { get; set; } // 技能的拥有者
        

    }

    public interface IPassiveSkill
    {
        void ApplyPassive();
    }
    public interface IActiveSkill
    {
        void AddEnergy(float amount);
        bool IsReady();
        bool Execute();
    }
}