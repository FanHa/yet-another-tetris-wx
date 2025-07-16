using Model.Tetri;

namespace Units.Skills
{
    public class HitAndRun : Skill
    {
        public HitAndRunConfig Config { get; }
        private bool hasTriggered;

        public HitAndRun(HitAndRunConfig config)
        {
            Config = config;
            hasTriggered = false;
        }

        public override CellTypeId CellTypeId => CellTypeId.HitAndRun;

        public override string Name() => "走A";
        public override string Description() => "和敌人保持攻击距离";
        public override bool IsReady()
        {
            return !hasTriggered;
        }


        protected override void ExecuteCore(Unit caster)
        {
            hasTriggered = true;
            caster.AddBuff(new Buffs.HitAndRunBuff(
                duration: -1f,
                sourceUnit: caster,
                sourceSkill: this
            ));
        }
    }
}