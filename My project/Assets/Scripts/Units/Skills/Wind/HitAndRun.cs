using Model.Tetri;

namespace Units.Skills
{
    public class HitAndRun : Skill, IPassiveSkill
    {
        public HitAndRunConfig Config { get; }

        public HitAndRun(HitAndRunConfig config)
        {
            Config = config;
        }

        public override CellTypeId CellTypeId => CellTypeId.HitAndRun;

        public override string Description()
        {
            return DescriptionStatic();
        }
        public static string DescriptionStatic() => "和敌人保持攻击距离,自动走A.";

        public override string Name()
        {
            return NameStatic();
        }
        public static string NameStatic() => "走A";


        public void ApplyPassive()
        {
            Owner.AddBuff(new Buffs.HitAndRunBuff(
                duration: -1f,
                sourceUnit: Owner,
                sourceSkill: this
            ));
        }
    }
}