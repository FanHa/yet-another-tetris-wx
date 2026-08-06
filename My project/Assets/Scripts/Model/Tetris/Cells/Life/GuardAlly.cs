using Units;

namespace Model.Tetri
{
    public class GuardAlly : Cell
    {
        public override AffinityType Affinity => AffinityType.Life;

        public override string Description()
        {
            return Units.Skills.GuardAlly.DescriptionStatic();
        }

        public override string Name()
        {
            return Units.Skills.GuardAlly.NameStatic();
        }

        public override void Apply(Unit unit)
        {
            unit.AddSkill(new Units.Skills.GuardAlly());
        }
    }
}
