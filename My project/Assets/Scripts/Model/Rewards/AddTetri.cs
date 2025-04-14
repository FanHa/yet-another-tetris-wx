using System;
using Controller;
using UnityEngine;

namespace Model.Rewards
{
    public class AddTetri: Reward
    {
        protected Model.Tetri.Tetri tetriInstance;
        protected Model.Tetri.Cell cellTemplate;

        public AddTetri(Model.Tetri.Tetri tetri, Model.Tetri.Cell cell)
        {
            tetriInstance = tetri;
            cellTemplate = cell;
            SetRandomCell();
        }

        public Model.Tetri.Tetri GetTetri()
        {
            return tetriInstance;
        }


        public override string Name() => cellTemplate.Name();
        public override string Description() => cellTemplate.Description();

        private void SetRandomCell()
        {
            var occupiedPositions = tetriInstance.GetOccupiedPositions();
            if (occupiedPositions.Count > 0)
            {
                var random = new System.Random();
                var randomPosition = occupiedPositions[random.Next(occupiedPositions.Count)];
                tetriInstance.SetCell(randomPosition.x, randomPosition.y, cellTemplate);
            }
        }
    }
}