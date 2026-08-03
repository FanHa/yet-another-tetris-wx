using System;
using Units.Skills;
using UnityEngine;
using UnityEngine.Serialization;

namespace Model.Tetri
{
    [Serializable]
    public sealed class CellSkillBindingItem
    {
        [SerializeField] private string cellId;
        [SerializeField] private SkillDefinition skillDefinition;

        public string CellId => cellId;
        public SkillDefinition SkillDefinition => skillDefinition;
    }
}
