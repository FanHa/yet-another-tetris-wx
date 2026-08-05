using System;
using Units.Skills;
using UnityEngine;

namespace Model.Tetri
{
    [Serializable]
    public sealed class CellSkillBindingItem
    {
        [SerializeField] private CellDefinition cellDefinition;
        [SerializeField] private SkillDefinition skillDefinition;

        public CellDefinition CellDefinition => cellDefinition;
        public SkillDefinition SkillDefinition => skillDefinition;
    }
}
