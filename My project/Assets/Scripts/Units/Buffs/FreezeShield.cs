using System;
using Units.Damages;
using UnityEngine;

namespace Units.Buffs
{
    public class FreezeShield : Buff
    {
        public FreezeShield()
        {
            durationSeconds = 8f; // Freeze Debuff 持续时间
        }

        public override string Name()
        {
            return "冰霜护盾";
        }

        public override string Description()
        {
            return "被攻击时,给攻击者施加冰霜效果";
        }

        public override void Apply(Unit unit)
        {
            // 绑定 OnAttacked 事件，当单位被攻击时触发
            unit.OnDamageTaken += ApplyFreezeDebuff;
        }

        public override void Remove(Unit unit)
        {
            // 解绑 OnAttacked 事件
            unit.OnDamageTaken -= ApplyFreezeDebuff;
        }

        private void ApplyFreezeDebuff(Damage damage)
        {
            if (damage.Type == DamageType.Hit) {
                Buff freezeDebuff = new Freeze();
                // 给攻击者添加 Freeze Debuff
                damage.SourceUnit.AddBuff(freezeDebuff);
            }


            
        }

        public override void Affect(Unit unit)
        {
            
        }

        public override Type TetriCellType => typeof(Model.Tetri.Skills.FreezeShield); // Return the Type of the corresponding TetriCell
    }
}