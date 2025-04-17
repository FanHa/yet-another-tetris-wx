namespace Units
{
    public interface ITakeDamageBehavior
    {
        Damages.Damage ModifyDamage(Damages.Damage damage);
    }
}
