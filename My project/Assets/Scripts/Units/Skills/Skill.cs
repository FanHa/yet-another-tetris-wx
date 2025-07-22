using System;
using System.Collections.Generic;
using System.Linq;
using Model.Tetri;
using UnityEngine;

namespace Units.Skills
{
    public abstract class Skill
    {

        public virtual CellTypeId CellTypeId => CellTypeId.None; // 默认值，子类可以覆盖

        public abstract string Name();
        public abstract string Description();

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
            if (ExecuteCore(caster))
            {
                CurrentEnergy -= RequiredEnergy; // 执行技能后消耗能量
            }
            else
            {
                CurrentEnergy -= RequiredEnergy * 0.5f; // 执行失败时消耗一半能量
                Debug.Log($"{Name()} 技能施放失败，消耗一半能量。单位：{caster.name}");
            }
        }


        protected abstract bool ExecuteCore(Unit caster); 

    }
}