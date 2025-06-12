using Units.Skills;
using UnityEngine;

namespace Units.Buffs
{
    /// <summary>
    /// 冰霜反击护盾：被攻击时对攻击者施加Chilled（多重减速）Debuff
    /// </summary>
    public class IceShield : Buff, ITakeHitTrigger
    {
        private float chilledDuration;         // Chilled Buff 持续时间（秒）
        private int chilledMoveSlowPercent;  // Chilled 移动速度减缓百分比
        private int chilledAttackSlowPercent;// Chilled 攻击速度减缓百分比
        private int chilledEnergyRegenSlowPercent; // Chilled 能量回复减缓百分比

        public IceShield(
            float buffDuration,
            float chilledDuration,
            int chilledMoveSlowPercent,
            int chilledAttackSlowPercent,
            int chilledEnergyRegenSlowPercent,
            Unit sourceUnit,
            Skill sourceSkill
        ) : base(buffDuration, sourceUnit, sourceSkill)
        {
            this.chilledDuration = chilledDuration;
            this.chilledMoveSlowPercent = chilledMoveSlowPercent;
            this.chilledAttackSlowPercent = chilledAttackSlowPercent;
            this.chilledEnergyRegenSlowPercent = chilledEnergyRegenSlowPercent;
        }

        public override string Name() => "冰霜护盾";
        public override string Description() =>
            $"被攻击时使攻击者获得Chilled：移速-{chilledMoveSlowPercent}%，攻速-{chilledAttackSlowPercent}%，能量回复-{chilledEnergyRegenSlowPercent}%，持续{chilledDuration}秒";

        public void OnTakeHit(Unit self, Unit attacker, ref Damages.Damage damage)
        {
            if (attacker == null)
                return;
            var chilled = new Chilled(
                chilledDuration,
                chilledMoveSlowPercent,
                chilledAttackSlowPercent,
                chilledEnergyRegenSlowPercent,
                self,
                sourceSkill
            );
            attacker.AddBuff(chilled);
        }
    }
}