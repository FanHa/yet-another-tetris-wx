using System;
using System.Linq;
using UnityEngine;

namespace Model.Rewards
{
    public abstract class UpgradeTetri : Reward
    {
        public Model.Tetri.Tetri TargetTetri { get; protected set; }
        public Model.Tetri.Tetri UpgradedTetri { get; protected set; }
        private readonly string name;
        private readonly string description;

        public UpgradeTetri(Model.Tetri.Tetri targetTetri)
        {
            this.TargetTetri = targetTetri;

            // 假设升级后 Tetri 里有一个新 Cell（非 Padding/Empty），用它来生成 name/description
            Tetri.Cell mainCell = TargetTetri.GetMainCell();
            if (mainCell != null)
            {
                name = $"升级方块：{mainCell.GetType().Name}";
                description = $"将一个填充物升级为 {mainCell.GetType().Name}";
            }
            else
            {
                name = "升级方块";
                description = "升级一个填充物";
            }

            // 移除参数，派生类会负责初始化 PreviewUpgradedTetri
        }


        public override string Name() => name;
        public override string Description() => description;

        public override string Apply(TetriInventoryModel tetriInventoryData)
        {
            throw new NotImplementedException();
        }
    }
}