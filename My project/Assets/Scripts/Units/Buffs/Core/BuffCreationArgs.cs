namespace Units.Buffs
{
    public interface IBuffCreationArgs
    {
        BuffSource Source { get; }
    }

    public readonly struct BuffSource
    {
        public Unit SourceUnit { get; }
        public Skills.Skill SourceSkill { get; }

        public BuffSource(Unit sourceUnit, Skills.Skill sourceSkill)
        {
            SourceUnit = sourceUnit;
            SourceSkill = sourceSkill;
        }
    }
}