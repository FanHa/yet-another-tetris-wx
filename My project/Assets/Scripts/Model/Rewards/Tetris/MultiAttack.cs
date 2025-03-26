
using UnityEngine;

namespace Model.Rewards
{
    public class MultiAttack : Tetri
    {
        public override string GetName() => "Multi Attack";
        public override string GetDescription() => "Increases attack target number";

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