using System;

namespace Model.Rewards
{
    public class UpgradeCoreCellTetri : UpgradeTetri
    {
        public UpgradeCoreCellTetri(Model.Tetri.Tetri targetTetri) : base(targetTetri)
        {
            // 预览升级后的 Tetri
            UpgradedTetri = CreatePreviewUpgradedTetri();
        }

        public override string Name() => "升级核心方块";
        public override string Description() => "将核心方块升级为更高级的核心方块";

        public override string Apply(TetriInventoryModel tetriInventoryData)
        {
            // 实现核心方块升级逻辑
            // ...升级核心方块的代码...
            throw new NotImplementedException();
        }

        protected  Model.Tetri.Tetri CreatePreviewUpgradedTetri()
        {
            Tetri.Tetri clone = TargetTetri.Clone();
            Tetri.Cell mainCell = clone.GetMainCell();
            // 这里应返回一个升级了核心方块的 Tetri 实例
            // var clone = TargetTetri.Clone();
            // clone.UpgradeCoreCell();
            // return clone;
            return TargetTetri; // TODO: 替换为实际升级后的 Tetri
        }
    }
}

