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

        public Unit Owner { get; set; } // 技能的拥有者
        

    }

    public interface IPassiveSkill
    {
        void ApplyPassive();
    }
    public interface IActiveSkill
    {
        void AddEnergy(float amount);
        bool IsReady();
        void Execute();
    }
}