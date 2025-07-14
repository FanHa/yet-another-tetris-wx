using Model.Tetri;

namespace Units.Skills
{
    public class WindShift : Skill
    {
        public WindShiftConfig Config { get; }
        private bool hasTriggered;
        public override CellTypeId CellTypeId => CellTypeId.WindShift;

        public WindShift(WindShiftConfig config)
        {
            hasTriggered = false;
            Config = config;
        }
        public override string Description()
        {
            return "将单位的形态切换为风形态，提升攻击距离, 降低伤害, 提升自身受到伤害";
        }

        public override string Name()
        {
            return "风形态";
        }
        public override bool IsReady()
        {
            return !hasTriggered;
        }

        protected override void ExecuteCore(Unit caster)
        {
            hasTriggered = true;
            caster.AddBuff(new Buffs.WindShiftBuff(
                duration: -1f,
                sourceUnit: caster,
                sourceSkill: this
            ));
        }
    }
}