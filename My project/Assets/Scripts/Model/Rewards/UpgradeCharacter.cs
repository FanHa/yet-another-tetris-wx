using System;
using UnityEngine;

namespace Model.Rewards
{
    public class UpgradeCharacter : Reward
    {
        private readonly Model.Tetri.Character targetCharacter;
        private readonly string name;
        private readonly string description;

        public UpgradeCharacter(Model.Tetri.Character character)
        {
            targetCharacter = character;
            name = $"升级角色：{character.GetType().Name}";
            description = $"将 {character.GetType().Name} 的等级提升至 {character.Level + 1}";
        }

        public override string Name() => name;
        public override string Description() => description;

        public override string Apply(Model.TetriInventoryModel tetriInventoryData)
        {
            throw new NotImplementedException();
        }
    }
}