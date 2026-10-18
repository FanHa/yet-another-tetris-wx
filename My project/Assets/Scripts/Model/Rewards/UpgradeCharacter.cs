using Model.Tetri;

namespace Model.Rewards
{
    public class UpgradeCharacter : Reward
    {
        public CharacterTetri TargetTetri { get; }
        private readonly string name;
        private readonly string description;

        public UpgradeCharacter(CharacterTetri characterTetri)
        {
            TargetTetri = characterTetri;
            name = $"升级角色：{TargetTetri.Character.Name()}";
            description = $"提升角色等级";

        }

        public override string Name() => name;
        public override string Description() => description;

        public override void Apply(Model.TetriInventoryModel tetriInventoryData)
        {
            TargetTetri.Upgrade();
        }
    }
}