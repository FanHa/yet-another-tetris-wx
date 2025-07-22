using System.Linq;
using Model.Tetri;
using UnityEngine;

namespace Units.Skills
{
    public class LifeShield : Skill
    {
        public override CellTypeId CellTypeId => CellTypeId.LifeShield;
        public LifeShieldConfig Config { get; }

        public LifeShield(LifeShieldConfig config)
        {
            Config = config;
            RequiredEnergy = config.RequiredEnergy;
        }

        protected override bool ExecuteCore(Unit caster)
        {
            // 找到所有友方单位（不包括自己）
            var target = caster.FindRandomAlly(float.MaxValue);

            // todo 尽量不要重复
            if (target == null)
                return false;

            float shieldAmount = caster.Attributes.CurrentHealth * (Config.LifeCostPercent / 100f);

            target.AddBuff(new Units.Buffs.LifeShieldBuff(
                shieldAmount,         // 护盾值
                Config.AbsorbPercent,       // 吸收百分比
                Config.BuffDuration,       // 持续时间
                caster,                     // 来源单位
                this                        // 来源技能
            ));
            return true;

        }

        public override string Description()
        {
            return DescriptionStatic();
        }
        public static string DescriptionStatic() => "随机为一个友方单位添加生命护盾，抵挡伤害。";

        public override string Name()
        {
            return NameStatic();
        }
        public static string NameStatic() => "生命护盾";
    }
}