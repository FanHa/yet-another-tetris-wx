using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Unity.VisualScripting;
using UnityEngine;

namespace Model.Tetri
{
    [Serializable]
    public abstract class Cell
    {

        public abstract string Description();
        public abstract string Name();

        public virtual void Apply(Units.Unit unit) { }
        public virtual void PostApply(Units.Unit unit, IReadOnlyList<Cell> allCells) { }

        public AffinityType Affinity { get; set; } = AffinityType.None;

    }
}