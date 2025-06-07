using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Units.Skills
{
    public abstract class Skill
    {

        public event Action<SkillEffectContext> OnEffectTriggered;

        public abstract string Name();

        public float RequiredEnergy;
        public float CurrentEnergy;
        
        public virtual bool IsReady()
        {
            return CurrentEnergy >= RequiredEnergy;
        }

        public virtual void AddEnergy(float amount)
        {
            CurrentEnergy += amount;
            if (CurrentEnergy > RequiredEnergy)
            {
                CurrentEnergy = RequiredEnergy; // 确保不会超过最大能量
            }
        }

        public virtual void Execute(Unit caster)
        {
            ExecuteCore(caster);
            CurrentEnergy -= RequiredEnergy; // 执行技能后消耗能量
        }


        protected abstract void ExecuteCore(Unit caster);

        public abstract string Description();

        protected void TriggerEffect(SkillEffectContext context)
        {
            OnEffectTriggered?.Invoke(context);
        }



        // todo 这些寻找方法似乎不该归Skill类管

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