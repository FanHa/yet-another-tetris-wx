namespace Units
{
    public class Burn : Buff
    {
        public int damagePerSecond = 2; // 每回合造成的伤害
        private float duration = 5f; // 持续时间

        public override void Apply(Unit target)
        {
            RefreshDuration();
        }
        
        public override string Name()
        {
            return "灼烧";
        }

        public override float Duration()
        {
            return duration;
        }

        public override string Description()
        {
            return $"{damagePerSecond} 伤害/秒, 持续{duration}秒";
        }

        public override void Remove(Unit unit)
        {
            
        }

        public override void Affect(Unit target)
        {
            target.TakeDamage(damagePerSecond);
        }
    }
}