using Model.Tetri;

namespace Units.Skills
{
    public class AttackBoost : Skill
    {
        public AttackBoostConfig Config { get; }

        public AttackBoost(AttackBoostConfig config)
        {
            Config = config;
            this.RequiredEnergy = config.RequiredEnergy;
        }

        public override CellTypeId CellTypeId => CellTypeId.AttackBoost;

        public override string Name() => "攻击加速";
        public override string Description() => $"短时间内攻击速度提升{Config.AtkSpeedPercent}%";

        protected override bool ExecuteCore(Unit caster)
        {
            int windCellCount = caster.CellCounts.TryGetValue(AffinityType.Wind, out var count) ? count : 0;
            float atkSpeedPercent = Config.AtkSpeedPercent + windCellCount * Config.AtkSpeedAdditionPercentPerWindCell;
            caster.AddBuff(new Buffs.AttackBoostBuff(
                duration: Config.Duration,
                atkSpeedPercent: atkSpeedPercent,
                sourceUnit: caster,
                sourceSkill: this
            ));
            return true;
        }
    }
}