
using UnityEngine;

namespace Model.Rewards
{
    public class MultiAttack : Tetri
    {
        private Model.Tetri.MultiAttack cellTemplate = new Model.Tetri.MultiAttack();
        public override string GetName() => cellTemplate.Name();
        public override string GetDescription() => cellTemplate.Description();

        public override void FillCells()
        {
            
            var occupiedPositions = tetriInstance.GetOccupiedPositions();
            if (occupiedPositions.Count > 0)
            {
                var random = new System.Random();
                var randomPosition = occupiedPositions[random.Next(occupiedPositions.Count)];
                tetriInstance.SetCell(randomPosition.x, randomPosition.y, new Model.Tetri.MultiAttack());
            }
        }
    }
}