using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Units.Skills
{
    public abstract class Skill
    {
        public abstract string Name();
        protected float lastUsedTime = 0f;

        public virtual float cooldown { get; protected set; } = 0f;
        
        public bool IsReady()
        {
            return Time.time >= lastUsedTime + cooldown;
        }

        public void Execute(Unit caster)
        {
            ExecuteCore(caster);
            lastUsedTime = Time.time; // 自动更新上次使用时间
        }

        protected abstract void ExecuteCore(Unit caster);

        public abstract string Description();

        public void Init(){
            // 初始化技能
            lastUsedTime = Time.time;
        }

        /// <summary>
        /// 寻找范围内的所有友方单位
        /// </summary>
        protected List<Unit> FindAlliesInRange(Unit caster, float range)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(caster.transform.position, range);
            return colliders
                .Select(collider => collider.GetComponent<Unit>())
                .Where(unit => unit != null && unit.faction == caster.faction)
                .ToList();
        }

        /// <summary>
        /// 寻找范围内的所有敌方单位
        /// </summary>
        protected List<Unit> FindEnemiesInRange(Unit caster, float range)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(caster.transform.position, range);
            return colliders
                .Select(collider => collider.GetComponent<Unit>())
                .Where(unit => unit != null && unit.faction != caster.faction)
                .ToList();
        }

        /// <summary>
        /// 寻找范围内最近的敌方单位
        /// </summary>
        protected Unit FindClosestEnemy(Unit caster, float range)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(caster.transform.position, range);
            return colliders
                .Select(collider => collider.GetComponent<Unit>())
                .Where(unit => unit != null && unit.faction != caster.faction)
                .OrderBy(unit => Vector2.Distance(caster.transform.position, unit.transform.position))
                .FirstOrDefault();
        }

        /// <summary>
        /// 寻找范围内的随机友方单位
        /// </summary>
        protected Unit FindRandomAlly(Unit caster, float range)
        {
            var allies = FindAlliesInRange(caster, range);
            if (allies.Count > 0)
            {
                return allies[UnityEngine.Random.Range(0, allies.Count)];
            }
            return null;
        }
        

    }
}