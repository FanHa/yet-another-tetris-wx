using System;
using Model.Resonance;
using UnityEngine;

namespace Model.Tetri
{
    [Serializable]
    public class Cell
    {
        private int level = 1;
        private CellDefinition definition;
        private ResonanceType resonance = ResonanceType.None;

        public int Level
        {
            get => level;
            set => level = Mathf.Max(1, value);
        }


        public CellDefinition Definition => definition;
        public ResonanceType Resonance
        {
            get => resonance;
            set => resonance = value;
        }
        public virtual Sprite Icon => definition.Icon;

        public virtual string Description() => definition.Description;
        public virtual string Name() => definition.DisplayName;

        public virtual void Initialize(CellDefinition cellDefinition)
        {
            definition = cellDefinition ?? throw new ArgumentNullException(nameof(cellDefinition));
            resonance = definition.Resonance;
        }

    }
}