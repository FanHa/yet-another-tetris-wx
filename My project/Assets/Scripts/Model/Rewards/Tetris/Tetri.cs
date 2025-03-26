using Controller;
using UnityEngine;

namespace Model.Rewards
{
    public abstract class Tetri : Reward
    {
        protected Model.Tetri.Tetri tetriInstance;

        public Model.Tetri.Tetri GetTetri()
        {
            return tetriInstance;
        }

        public void SetTetri(Model.Tetri.Tetri tetri)
        {
            tetriInstance = tetri;
        }

        public abstract void FillCells();
    }
}