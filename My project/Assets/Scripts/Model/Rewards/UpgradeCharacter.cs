using System;
using System.Linq;
using UnityEngine;

namespace Model.Rewards
{
    public class UpgradeCharacter : Reward
    {
        public Model.Tetri.Tetri TargetTetri { get; protected set; }
        public Model.Tetri.Tetri UpgradedTetri { get; protected set; }
        private readonly Model.Tetri.Character targetCharacter;
        private readonly string name;
        private readonly string description;

        public UpgradeCharacter(Model.Tetri.Tetri characterTetri)
        {
            TargetTetri = characterTetri;
            targetCharacter = TargetTetri.GetOccupiedPositions()
                .Select(pos => TargetTetri.Shape[pos.x, pos.y])
                .OfType<Model.Tetri.Character>()
                .FirstOrDefault();

            Model.Tetri.Tetri clone = TargetTetri.Clone();
            if (targetCharacter != null)
            {
                // 在克隆体中找到对应的 Character，并升级
                var clonedCharacter = clone.GetOccupiedPositions()
                    .Select(pos => clone.Shape[pos.x, pos.y])
                    .OfType<Model.Tetri.Character>()
                    .FirstOrDefault();

                if (clonedCharacter != null)
                {
                    clonedCharacter.Level += 1;
                }
                UpgradedTetri = clone;
            }
            else
            {
                UpgradedTetri = TargetTetri; // 如果没有角色，直接使用原 Tetri
            }

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

        public override string Apply(Model.TetriInventoryModel tetriInventoryData)
        {
            throw new NotImplementedException();
        }
    }
}