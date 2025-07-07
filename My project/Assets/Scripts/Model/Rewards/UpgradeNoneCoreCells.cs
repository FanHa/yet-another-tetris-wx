using System;
using System.Linq;
using Model.Tetri;

namespace Model.Rewards
{
    public class UpgradeNoneCoreCells : UpgradeTetri
    {
        public UpgradeNoneCoreCells(Model.Tetri.Tetri targetTetri, TetriFactory tetriFactory) 
            : base(targetTetri)
        {
            Model.Tetri.Tetri previewTetri = tetriFactory.Clone(targetTetri);
            previewTetri.UpgradeNoneCoreCells();
            UpgradedTetri = previewTetri;

        }

        public override string Name() => "升级非核心方块";
        public override string Description() => "将一个非核心方块升级为更高级的方块";

        public override void Apply(TetriInventoryModel tetriInventoryData)
        {
            TargetTetri.UpgradeNoneCoreCells();
        }

    }
}

