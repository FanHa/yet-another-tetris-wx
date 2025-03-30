namespace Units
{
    public class Burn : Debuff
    {
        public int damagePerSecond = 2; // 每回合造成的伤害
        private float duration = 5f; // 持续时间

        public override void ApplyEffect(Unit target)
        {
            target.TakeDamage(damagePerSecond);
        }
        
        public override string Name()
        {
            return "Burn";
        }

        public override float Duration()
        {
            return duration; // 持续时间为5秒
        }

        public override string Description()
        {
            return $"Burn: {damagePerSecond} damage per turn for {duration} seconds.";
        }
    }
}