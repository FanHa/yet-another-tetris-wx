namespace Units
{
    public interface ITakeDamageBehavior
    {
        float ModifyDamage(Unit source, float damage); // 修改伤害值，增加伤害来源
    }
}
