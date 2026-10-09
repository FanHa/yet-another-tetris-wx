using Units.Skills;
using UnityEngine;

namespace Units.Buffs
{
    public readonly struct LifeShieldBuffArgs : IBuffCreationArgs
    {
        public float ShieldValue { get; }
        public float Duration { get; }
        public BuffSource Source { get; }

        public LifeShieldBuffArgs(float shieldValue, float duration, BuffSource source)
        {
            ShieldValue = shieldValue;
            Duration = duration;
            Source = source;
        }
    }

    public class LifeShieldBuff : Buff
    {
        private float shieldValue;           // 当前护盾值
        private Shield shield;               // 护盾对象
        public LifeShieldBuff(Model.Buffs.BuffDefinition definition, LifeShieldBuffArgs args)
            : base(definition, args.Duration, args.Source.SourceUnit, args.Source.SourceSkill)
        {
            shieldValue = args.ShieldValue;
        }

        public override string Name() => "生命护盾";
        public override string Description() => $"受到伤害时吸收伤害，最多吸收{shieldValue}点，持续{duration}秒";

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