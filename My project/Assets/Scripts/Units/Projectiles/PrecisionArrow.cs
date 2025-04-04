namespace Units.Projectiles
{
    public class PrecisionArrow : Projectile
    {

        protected override void OnHitTarget()
        {
        // 对目标造成伤害
            var targetUnit = target.GetComponent<Unit>();
            if (targetUnit != null)
            {
                targetUnit.TakeDamage(damage);

            }
            Destroy(gameObject); // 销毁投射物
        }
    }
}