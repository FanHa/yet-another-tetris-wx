using System;

namespace Units.Skills
{
    public interface ISkillHandlerContext
    {
        Unit Owner { get; }
        float EnergyPerSecond { get; }
    }

    public sealed class SkillHandlerContext : ISkillHandlerContext
    {
        private readonly Unit owner;

        public SkillHandlerContext(Unit owner)
        {
            this.owner = owner ?? throw new ArgumentNullException(nameof(owner));
        }

        public Unit Owner => owner;
        public float EnergyPerSecond => owner.Attributes.EnergyPerSecond.finalValue;
    }
}
