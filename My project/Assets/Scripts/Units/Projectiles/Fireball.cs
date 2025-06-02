namespace Units.Projectiles
{
    public class Fireball : Projectile
    {
        private Dot dot; // 火焰持续伤害
        public void SetDot(Dot dot)
        {
            this.dot = dot;
        }

        protected override void HandleHitTarget()
        {
            if (target == null)
            {
                Destroy(gameObject);
                return;
            }

            Unit unit = target.GetComponent<Units.Unit>();
            if (unit == null)
            {
                Destroy(gameObject);
                return;
            }

            unit.TakeDamage(damage); // 对目标单位造成伤害
            // 处理火焰持续伤害
            if (dot != null)
            {
                unit.ApplyDot(dot);
            }
            Destroy(gameObject); // 销毁投射物
        }
    }
}