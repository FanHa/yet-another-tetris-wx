using System;
using System.Collections.Generic;

namespace Units.Buffs
{
    public class Burn : Buff
    {
        public Units.Unit source; // 施加灼烧效果的单位
        private int damagePerSecond = 2; // 每秒伤害

        public Burn()
        {
            durationSeconds = 5f; // 持续时间
        }

        public override void Apply(Unit target)
        {

        }
        
        public override string Name()
        {
            return "灼烧";
        }

        public override string Description()
        {
            return $"{damagePerSecond} 伤害每秒, 持续{durationSeconds}秒";
        }

        public override void Remove(Unit unit)
        {
            
        }

        public override void Affect(Unit target)
        {
            target.TakeDamage(new Damages.Damage(
                damagePerSecond,
                Name(),
                Damages.DamageType.Skill,
                source,
                target,
                new List<Buffs.Buff>())
            ); // 施加伤害
        }

        public override Type TetriCellType => typeof(Model.Tetri.Burn); // Return the Type of the corresponding TetriCell
    }
}