using System;
using UnityEngine;

namespace Model.Tetri
{
    [Serializable]
    public class TetriCellCharacter : TetriCell
    {
        public enum CharacterType
        {
            Square,
            Circle,
            Triangle,
            Star,
            Fire,
            Ice,
            // 其他类型...
        }

        [SerializeField]
        public CharacterType characterType; // 角色类型
        public TetriCellCharacter(CharacterType characterType)
        {
            type = CellType.CombatUnit;
            this.characterType = characterType;

        }

        public override object Clone()
        {
            return new TetriCellCharacter(characterType);
        }
    }
}