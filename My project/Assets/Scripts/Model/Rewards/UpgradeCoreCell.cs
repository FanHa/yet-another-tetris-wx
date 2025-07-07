using System;
using Model.Tetri;

namespace Model.Rewards
{
    public class UpgradeCoreCell : UpgradeTetri
    {
        public UpgradeCoreCell(Model.Tetri.Tetri targetTetri)
            : base(targetTetri)
        {

        }

        public override string Name() => "升级核心方块";
        public override string Description() => "将核心方块升级为更高级的核心方块";

        public override void Apply(TetriInventoryModel tetriInventoryData)
        {
            TargetTetri.UpgradeCoreCell(); 
        }
    }
}

