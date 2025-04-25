using System;
using System.Linq;
using UnityEngine;

namespace Model.Rewards
{
    public class UpgradeTetri : Reward
    {
        private readonly Model.Tetri.Tetri targetTetri; // 目标 Tetri
        private readonly Vector2Int targetPosition;    // 目标位置
        private readonly Model.Tetri.Cell newCell; 

        public UpgradeTetri(Model.Tetri.Tetri targetTetri, Vector2Int targetPosition, Model.Tetri.Cell newCell)
        {
            this.targetTetri = targetTetri;
            this.targetPosition = targetPosition;
            this.newCell = newCell;
        }


        public  void Apply()
        {
            // 将 PaddingCell 替换为指定类型的 Cell
            targetTetri.SetCell(targetPosition.x, targetPosition.y, newCell);
        }


        public override string Name() => $"升级 {newCell.Name()}" + targetTetri.Group;
        public override string Description() => $"升级一个填充物成为 {newCell.Name()}";
        public Model.Tetri.Tetri GetTargetTetri() => targetTetri;

        public Vector2Int GetTargetPosition() => targetPosition;

        public Model.Tetri.Cell GetNewCell() => newCell;
    }
}