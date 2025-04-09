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

        public void SetRandomCell<T>() where T : Model.Tetri.Cell, new()
        {
            var occupiedPositions = tetriInstance.GetOccupiedPositions();
            if (occupiedPositions.Count > 0)
            {
                var random = new System.Random();
                var randomPosition = occupiedPositions[random.Next(occupiedPositions.Count)];
                tetriInstance.SetCell(randomPosition.x, randomPosition.y, new T());
            }
        }

        public abstract void FillCells();
    }
}