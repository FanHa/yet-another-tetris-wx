using System;
using UnityEngine;

namespace Model.Tetri
{
    [Serializable]
    public class TetriCellAura : TetriCell
    {
        [SerializeField]
        public string auraEffect; // 光环效果

        public TetriCellAura(string auraEffect)
        {
            this.auraEffect = auraEffect;
        }

        public override object Clone()
        {
            return new TetriCellAura(auraEffect);
        }
    }
}