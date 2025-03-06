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
            this.skillName = skillName;
        }

        public override string Description()
        {
            throw new NotImplementedException();
        }

    }
}