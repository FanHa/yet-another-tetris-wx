using Model.Tetri;

namespace Units.Skills
{
    public class WindShift : Skill, IPassiveSkill
    {
        public WindShiftConfig Config { get; }
        public override CellTypeId CellTypeId => CellTypeId.WindShift;

        public WindShift(WindShiftConfig config)
        {
            Config = config;
        }

        private struct WindShiftStats
        {
            public StatValue AttackRangeBonus;
        }

        private WindShiftStats CalcStats()
        {
            int windCellCount = Owner != null && Owner.CellCounts.TryGetValue(AffinityType.Wind, out var count) ? count : 0;
            return new WindShiftStats
            {
                AttackRangeBonus = new StatValue(
                    "攻击距离提升",
                    Config.AttackRangeBonus,
                    windCellCount * Config.AttackRangePerWindCell
                )
            };
        }
        public override string Description()
        {
            var stats = CalcStats();
            return
                DescriptionStatic() + ":\n" +
                $"{stats.AttackRangeBonus}";
        }
 
        public static string DescriptionStatic() => "将单位的形态切换为风形态，提升攻击距离,获得攻击距离伤害加成";

        public override string Name()
        {
            return NameStatic();
        }
        public static string NameStatic() => "风形态";
    
        public void ApplyPassive()
        {
            var stats = CalcStats();
            Owner.AddBuff(new Buffs.WindShiftBuff(
                duration: -1f,
                sourceUnit: Owner,
                sourceSkill: this,
                attackRangeBonus: stats.AttackRangeBonus.Final
            ));
        }
        
    }
}