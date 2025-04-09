using System;
using UnityEngine;

namespace Units.Buffs
{
    public class FreezeShield : Buff
    {
        private float duration = 8f; // Freeze Debuff 持续时间

        public override string Name()
        {
            return "冰霜护盾";
        }

        public override float Duration()
        {
            return duration; // Buff 的持续时间
        }

        public override string Description()
        {
            return $"被攻击时,给攻击者施加冰霜效果";
        }

        public override void Apply(Unit unit)
        {
            // 绑定 OnAttacked 事件，当单位被攻击时触发
            unit.OnAttacked += ApplyFreezeDebuff;
        }

        public override void Remove(Unit unit)
        {
            // 解绑 OnAttacked 事件
            unit.OnAttacked -= ApplyFreezeDebuff;
        }

        private void ApplyFreezeDebuff(Unit attacker)
        {
            if (attacker == null) return;

            // 创建 Freeze Debuff
            Buff freezeDebuff = new Freeze();

            // 给攻击者添加 Freeze Debuff
            attacker.AddBuff(freezeDebuff);
        }

        public override void Affect(Unit unit)
        {
            
        }
    }
}