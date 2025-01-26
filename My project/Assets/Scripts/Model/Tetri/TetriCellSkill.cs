using System;
using UnityEngine;

namespace Model.Tetri
{
    [Serializable]
    public class TetriCellSkill : TetriCell
    {
        [SerializeField]
        public string skillName; // 技能名称

        public TetriCellSkill(string skillName)
        {
            type = CellType.Skill;
            this.skillName = skillName;
        }

        public override object Clone()
        {
            return new TetriCellSkill(skillName);
        }
    }
}