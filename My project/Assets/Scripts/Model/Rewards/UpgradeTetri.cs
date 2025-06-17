using System;
using System.Linq;
using UnityEngine;

namespace Model.Rewards
{
    public class UpgradeTetri : Reward
    {
        private readonly Model.Tetri.Tetri targetTetri; // 目标 Tetri
        private readonly string name;
        private readonly string description;

        public UpgradeTetri(Model.Tetri.Tetri targetTetri)
        {
            this.targetTetri = targetTetri;

            // 假设升级后 Tetri 里有一个新 Cell（非 Padding/Empty），用它来生成 name/description
            var upgradedCell = targetTetri.GetOccupiedPositions()
                .Select(pos => targetTetri.Shape[pos.x, pos.y])
                .FirstOrDefault(cell => !(cell is Model.Tetri.Padding) && !(cell is Model.Tetri.Empty));

            if (upgradedCell != null)
            {
                name = $"升级方块：{upgradedCell.GetType().Name}";
                description = $"将一个填充物升级为 {upgradedCell.GetType().Name}";
            }
            else
            {
                name = "升级方块";
                description = "升级一个填充物";
            }
        }

        public override string Name() => name;
        public override string Description() => description;

        public override string Apply(TetriInventoryModel tetriInventoryData)
        {
            throw new NotImplementedException();
        }
    }
}