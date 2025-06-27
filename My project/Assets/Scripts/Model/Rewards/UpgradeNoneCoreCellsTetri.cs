using System;
using System.Linq;

namespace Model.Rewards
{
    public class UpgradeNoneCoreCellsTetri : UpgradeTetri
    {
        public UpgradeNoneCoreCellsTetri(Model.Tetri.Tetri targetTetri) : base(targetTetri)
        {
            GenerateUpgradedTetri();
        }

        public override string Name() => "升级非核心方块";
        public override string Description() => "将一个非核心方块升级为更高级的方块";

        public override string Apply(TetriInventoryModel tetriInventoryData)
        {
            // 实现非核心方块升级逻辑
            // ...升级非核心方块的代码...
            throw new NotImplementedException();
        }

        private void GenerateUpgradedTetri()
        {
            // 1. 检查 TargetTetri 是否可升级
            if (TargetTetri.CanBeUpgraded() == false)
            {
                UpgradedTetri = TargetTetri;
                return;
            }

            // 2. 找到 Tetri 的主 Cell
            Model.Tetri.Cell mainCell = TargetTetri.GetMainCell();
            if (mainCell == null)
            {
                UpgradedTetri = TargetTetri;
                return;
            }

            // 3. 找到主 Cell 的 Affinity
            Model.Tetri.AffinityType mainCellAffinity = mainCell.Affinity;

            // 4. 升级 Padding
            // 克隆 Tetri
            Tetri.Tetri clone = TargetTetri.Clone();

            Tetri.AffinityType[] affinities = Enum.GetValues(typeof(Model.Tetri.AffinityType))
                .Cast<Model.Tetri.AffinityType>()
                .Where(a => a != Model.Tetri.AffinityType.None)
                .ToArray();
            var random = new System.Random();
            for (int x = 0; x < clone.Shape.GetLength(0); x++)
            {
                for (int y = 0; y < clone.Shape.GetLength(1); y++)
                {
                    var cell = clone.Shape[x, y];
                    if (cell is Model.Tetri.Padding paddingCell)
                    {
                        // 如果主Cell的Affinity不是None，直接赋值
                        if (mainCellAffinity != Model.Tetri.AffinityType.None)
                        {
                            paddingCell.Affinity = mainCellAffinity;
                        }
                        else
                        {                            
                            paddingCell.Affinity = affinities[random.Next(affinities.Length)];
                        }
                    }
                }
            }

            UpgradedTetri = clone;
        }
    }
}

