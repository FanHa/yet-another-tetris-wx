using System;
using Units.Skills;
using UnityEngine;

namespace Model.Tetri
{
    [Serializable]
    public class Cell
    {
        private int level = 1;
        private CellDefinition definition;
        private AffinityType affinity = AffinityType.None;

        public int Level
        {
            get => level;
            set => level = Mathf.Max(1, value);
        }

        // Runtime primary id for the CellDefinition workflow.
        public virtual string CellId => definition.Id;

        public CellDefinition Definition => definition;
        public virtual Sprite Icon => definition.Icon;

        public virtual string Description() => definition.Description;
        public virtual string Name() => definition.DisplayName;

        public virtual void Initialize(CellDefinition cellDefinition)
        {
            definition = cellDefinition ?? throw new ArgumentNullException(nameof(cellDefinition));
            affinity = definition.Affinity;
        }

        public virtual void Apply(Units.Unit unit) { }
        public virtual AffinityType Affinity
        {
            get => affinity;
            set => affinity = value;
        }
    }
}