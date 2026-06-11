using Model.Tetri;

namespace Units.Skills
{
    public class GuardAlly : Skill, IPassiveSkill
    {
        public override CellTypeId CellTypeId => CellTypeId.GuardAlly;

        public override string Name() => NameStatic();
        public static string NameStatic() => "护卫姿态";

        public override string Description() => DescriptionStatic();
        public static string DescriptionStatic() => "将移动行为切换为优先靠近队友";

        public void ApplyPassive()
        {
            Owner.SetMoveBehaviorMode(Unit.MoveBehaviorMode.TowardAlly);
        }
    }
}
