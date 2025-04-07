using System.Collections.Generic;

namespace Units
{
    public class Burn : Buff
    {
        public Units.Unit source; // 施加灼烧效果的单位
        private int damagePerSecond = 2; // 每秒伤害
        private float duration = 5f; // 持续时间

        public override void Apply(Unit target)
        {

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
            return $"攻击附带灼烧效果, 造成{damagePerSecond} 伤害每秒, 持续{duration}秒";
        }

        public override void Remove(Unit unit)
        {
            
        }

        public override void Affect(Unit target)
        {
            target.TakeDamage(source, new Damages.Damage(damagePerSecond, Name(), false)); // 施加伤害
        }
    }
}