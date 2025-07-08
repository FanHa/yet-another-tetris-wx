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

        public override string Name() => "升级填充方块";
        public override string Description() => "赋予填充方块属性,对核心方块提供加成";

        public override void Apply(TetriInventoryModel tetriInventoryData)
        {
            TargetTetri.UpgradeNoneCoreCells();
        }

    }
}

