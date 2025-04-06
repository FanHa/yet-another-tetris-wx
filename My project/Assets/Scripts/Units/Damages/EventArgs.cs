namespace Units.Damages
{
    public class EventArgs
    {
        public Unit Source { get; } // 伤害来源
        public Unit Target { get; } // 伤害目标
        public Damage Damage { get; } // 伤害类型
        public EventArgs(Unit source, Unit target, Damage damageReceived)
        {
            Source = source;
            Target = target;
            Damage = damageReceived;
        }
    }
}