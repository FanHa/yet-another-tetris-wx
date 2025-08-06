using Units.Skills;

namespace Units.Buffs
{
    public class HitAndRunBuff : Buff
    {
        public HitAndRunBuff(float duration, Unit sourceUnit, Skill sourceSkill)
            : base(duration, sourceUnit, sourceSkill) { }

        public override void OnApply(Unit unit)
        {
            base.OnApply(unit);
            unit.SetHitAndRun(true);
        }

        public override void OnRemove()
        {
            owner.SetHitAndRun(false);
            base.OnRemove();
        }

        public override string Name() => "走A";
        public override string Description() => "和敌人保持攻击距离";
    }
}