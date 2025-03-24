using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
namespace Model.Reward
{
    public class RangeAttack : BaseReward
    {
        protected override void FillCells(Tetri.Tetri tetri)
        {
            var occupiedPositions = tetri.GetOccupiedPositions();
            if (occupiedPositions.Count > 0)
            {
                var random = new System.Random();
                var randomPosition = occupiedPositions[random.Next(occupiedPositions.Count)];
                tetri.SetCell(randomPosition.x, randomPosition.y, new Tetri.RangeAttack());
            }
        }

        protected override string GetName() => "Range Attack";
        protected override string GetDescription() => "Increases attack range";
    }
}