using System;
using Units.Skills;
using UnityEngine;

namespace Model.Tetri
{
    [Serializable]
    public sealed class CellSkillBindingItem
    {
        [SerializeField] private string cellId;
        [SerializeField] private SkillConfig skillConfig;

        public string CellId => cellId;
        public SkillConfig SkillConfig => skillConfig;
    }
}
