using Units.Buffs;

namespace Units
{
    public enum BuffChangeKind
    {
        Added,
        Removed
    }

    public readonly struct UnitBuffChangedEvent
    {
        public Unit Owner { get; }
        public Buff Buff { get; }
        public BuffChangeKind Kind { get; }

        public UnitBuffChangedEvent(Unit owner, Buff buff, BuffChangeKind kind)
        {
            Owner = owner;
            Buff = buff;
            Kind = kind;
        }
    }
}