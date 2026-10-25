using Model.Buffs;
using UnityEngine;

namespace Units.Buffs
{
    public class LifeShieldBuff : Buff
    {
        private readonly float shieldValue;
        private Shield shield;               // 护盾对象
        public LifeShieldBuff(BuffDefinition definition, float shieldValue, BuffDuration duration, BuffSource source)
            : base(definition, duration, source.SourceUnit, source.SourceSkill)
        {
            this.shieldValue = shieldValue;
        }

        public override string Name() => Definition.DisplayName;
        public override string DescribeAppliedEffect() =>
            $"受到伤害时吸收伤害，护盾总吸收量{shieldValue}，{(Duration.IsPermanent ? "永久" : $"持续{Duration.Seconds}秒")}";

        public override void OnApply(IBuffContext context)
        {
            base.OnApply(context);
            shield = new Shield(this, shieldValue);
            shield.OnBroken += OnShieldBroken;
            context.Attributes.AddShield(shield);
        }

        public override void OnRemove()
        {
            shield.OnBroken -= OnShieldBroken;
            context.Attributes.RemoveShield(shield);
            base.OnRemove();
        }

        private void OnShieldBroken(Shield shield)
        {
            // 护盾破碎时的处理逻辑
            // 例如：播放特效、移除Buff等
            context.RemoveBuff(this);
        }
    }
}