using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Units.Skills;
using Unity.VisualScripting;
using UnityEngine;

namespace Model.Tetri
{
    [Serializable]
    public abstract class Cell
    {
        private int level = 1;
        public int Level
        {
            get => level;
            set => level = Mathf.Max(1, value);
        }
        public virtual CellTypeId CellTypeId { get; }
        public SkillConfigGroup SkillConfigGroup;

        public abstract string Description();
        public abstract string Name();

        public virtual void Apply(Units.Unit unit) { }
        public virtual AffinityType Affinity
        {
            get => AffinityType.None;
            set { }
        }

    }
}