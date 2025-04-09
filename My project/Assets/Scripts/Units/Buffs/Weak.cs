using System;
using UnityEngine;

namespace Units.Buffs
{
    public class Weak : Buff
    {
        public float attackReductionPercentage = 10f; // 攻击力减少百分比
        public float damageTakenIncreasePercentage = 10f; // 受到的伤害增加百分比
        private float duration = 10f; // 持续时间
        private class DamageIncreaseBehavior : ITakeDamageBehavior
        {
            private float damageIncreasePercentage;

            public DamageIncreaseBehavior(float damageIncreasePercentage)
            {
                this.damageIncreasePercentage = damageIncreasePercentage;
            }

            public Damages.Damage ModifyDamage(Unit source, Damages.Damage damage)
            {
                // 增加伤害
                float increasedDamage = damage.Value * (1 + damageIncreasePercentage / 100f);
                return new Damages.Damage(increasedDamage, damage.DamageType, damage.CanBeReflected);
            }
        }
        private DamageIncreaseBehavior damageIncreaseBehavior;

        public override Type TetriCellType => typeof(Model.Tetri.Weak); // Return the Type of the corresponding TetriCell

        public override string Name()
        {
            return "虚弱";
        }

        public override float Duration()
        {
            return duration;
        }

        public override string Description()
        {
            return $"减少目标攻击力{attackReductionPercentage}%，增加目标受到的伤害{damageTakenIncreasePercentage}%，持续{duration}秒";
        }

        public override void Apply(Unit unit)
        {
            // 减少攻击力
            unit.attackPower.AddPercentageModifier(this, -attackReductionPercentage);

            // 增加受到的伤害
            damageIncreaseBehavior = new DamageIncreaseBehavior(damageTakenIncreasePercentage);
            unit.AddDamageBehavior(damageIncreaseBehavior);
        }

        public override void Remove(Unit unit)
        {
            // 恢复攻击力
            unit.attackPower.RemovePercentageModifier(this);

            // 移除增加伤害的行为
            if (damageIncreaseBehavior != null)
            {
                unit.RemoveDamageBehavior(damageIncreaseBehavior);
                damageIncreaseBehavior = null;
            }
        }

        public override void Affect(Unit unit)
        {
            // 虚弱效果不需要每秒触发，仅在 Apply 和 Remove 时生效
        }
    }
}