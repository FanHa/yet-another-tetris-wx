using System;
using System.Linq;
using Model.Tetri;
using UnityEngine;

namespace Model.Rewards
{
    public class UpgradeCharacter : Reward
    {
        public Model.Tetri.Tetri TargetTetri { get; protected set; }
        private readonly Model.Tetri.Character targetCharacter;
        private readonly string name;
        private readonly string description;

        public UpgradeCharacter(Model.Tetri.Tetri characterTetri)
        {
            TargetTetri = characterTetri;

            if (targetCharacter != null)
            {
                name = $"升级角色：{targetCharacter.GetType().Name}";
                description = $"将 {targetCharacter.GetType().Name} 的等级提升至 {targetCharacter.Level + 1}";
            }
            else
            {
                name = "升级角色";
                description = "提升角色等级";
            }
        }

        public override string Name() => name;
        public override string Description() => description;

        public override void Apply(Model.TetriInventoryModel tetriInventoryData)
        {
            TargetTetri.UpgradeCoreCell(); // 升级核心方块
        }
    }
}