using System;
using System.Linq;
using Model.Tetri;
using UnityEngine;

namespace Model.Rewards
{
    public class UpgradeCharacter : Reward
    {
        public Model.Tetri.Tetri TargetTetri { get; protected set; }
        private readonly string name;
        private readonly string description;

        public UpgradeCharacter(Model.Tetri.Tetri characterTetri)
        {
            TargetTetri = characterTetri;
            var characterCell = TargetTetri.GetMainCell() as Model.Tetri.Character;
            name = $"升级角色：{characterCell.Name()}";
            description = $"提升角色等级";

        }

        public override string Name() => name;
        public override string Description() => description;

        public override void Apply(Model.TetriInventoryModel tetriInventoryData)
        {
            TargetTetri.UpgradeCoreCell(); // 升级核心方块
        }
    }
}