namespace Units
{
    public interface ITakeDamageBehavior
    {
        Damages.Damage ModifyDamage(Unit source, Damages.Damage damage);
    }
}
