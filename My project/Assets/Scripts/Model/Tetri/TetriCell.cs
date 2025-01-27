using System;
using UnityEngine;

namespace Model.Tetri
{
    [Serializable]
    public abstract class TetriCell :ScriptableObject, ICloneable
    {
        public enum CellType
        {
            Empty,
            Basic,
            CombatUnit,
            IndividualAttribute,
            Aura,
            Skill,
            // 其他类型...
        }

        [SerializeField]
        public CellType type = CellType.Empty; // 类别

        public abstract object Clone();
    }
}