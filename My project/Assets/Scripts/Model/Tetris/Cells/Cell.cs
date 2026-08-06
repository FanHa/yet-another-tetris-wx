using System;
using Units.Skills;
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

        // Runtime primary id for the CellDefinition workflow.
        public virtual string CellId => GetType().Name;

        public SkillConfig Config;

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