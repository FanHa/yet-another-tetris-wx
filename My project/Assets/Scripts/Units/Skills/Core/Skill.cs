using System;
using System.Collections.Generic;
using System.Linq;
using Controller;
using Model.Tetri;
using Model.Skills;
using UnityEngine;

namespace Units.Skills
{
    public interface ISkillContext
    {
        Unit SelfUnit { get; }

        Attributes Attributes { get; }
        Dictionary<AffinityType, int> CellCounts { get; }

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

        public abstract string Name();
        public abstract string Description();

        public SkillDefinition Definition { get; internal set; }
        public ISkillContext Owner { get; set; } // 技能的拥有者
        

    }

    public interface IPassiveSkill
    {
        void ApplyPassive();
    }
}