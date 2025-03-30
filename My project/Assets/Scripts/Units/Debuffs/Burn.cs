namespace Units
{
    public class Burn : Debuff
    {
        public int damagePerTurn = 2; // 每回合造成的伤害
        public int duration = 5; // 持续回合数

        public override void ApplyEffect(Unit target)
        {
            target.TakeDamage(damagePerTurn);
        }

        public override string Description()
        {
            return $"Burn: {damagePerTurn} damage per turn for {duration} seconds.";
        }
    }
}