using UnityEngine;

namespace Units.Projectiles
{
    public class Snowball : Projectile
    {
        private Units.Buffs.Chilled chilledBuff;

        public void SetChilled(Units.Buffs.Chilled chilled)
        {
            this.chilledBuff = chilled;
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

            unit.TakeDamage(damage); // 对目标单位造成冰属性伤害

            // 施加冰霜减速Buff
            if (chilledBuff != null)
            {
                unit.AddBuff(chilledBuff);
            }

            Destroy(gameObject); // 销毁投射物
        }
    }
}