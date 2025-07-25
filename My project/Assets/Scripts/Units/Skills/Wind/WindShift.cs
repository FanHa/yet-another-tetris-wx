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
        public override string Description()
        {
            return DescriptionStatic();
        }
        public static string DescriptionStatic() => "将单位的形态切换为风形态，提升攻击距离，降低伤害，提升自身受到伤害。";

        public override string Name()
        {
            return NameStatic();
        }
        public static string NameStatic() => "风形态";
    
        public void ApplyPassive()
        {
            Owner.AddBuff(new Buffs.WindShiftBuff(
                duration: -1f,
                sourceUnit: Owner,
                sourceSkill: this,
                attackRangeBonus: Config.AttackRangeBonus,
                damageReducePercent: Config.DamageReducePercent,
                takeDamageIncreasePercent: Config.TakeDamageIncreasePercent
            ));
        }
        
    }
}