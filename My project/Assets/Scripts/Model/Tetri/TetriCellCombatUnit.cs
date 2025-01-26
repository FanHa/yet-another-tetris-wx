using System;
using UnityEngine;

namespace Model.Tetri
{
    [Serializable]
    public class TetriCellCombatUnit : TetriCell
    {
        [SerializeField]
        public string attribute; // 属性

        [SerializeField]
        public string specialSkill; // 特技

        public TetriCellCombatUnit(string attribute, string specialSkill)
        {
            type = CellType.CombatUnit;
            this.attribute = attribute;
            this.specialSkill = specialSkill;
        }

        public override object Clone()
        {
            return new TetriCellCombatUnit(attribute, specialSkill);
        }
    }
}