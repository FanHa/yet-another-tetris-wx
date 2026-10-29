using Model.Tetri;
using Model.Skills;

namespace Units.Skills
{
    public class GuardAlly : Skill
    {
        public override string Name() => Definition.DisplayName;

        public override string DescribeEffect() => Definition.Description;

        public override void OnPassiveEffectsApplication()
        {
            Owner.SetMoveBehaviorMode(Unit.MoveBehaviorMode.TowardAlly);
        }
    }
}
