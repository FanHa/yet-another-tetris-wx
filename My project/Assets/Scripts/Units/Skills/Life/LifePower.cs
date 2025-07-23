using Model.Tetri;
using UnityEngine;

namespace Units.Skills
{
    public class LifePower : ActiveSkill
    {
        public override CellTypeId CellTypeId => CellTypeId.LifePower; // 如果有专属ID请替换
        public LifePowerConfig Config { get; }

        public LifePower(LifePowerConfig config)
        {
            Config = config;
            RequiredEnergy = config.RequiredEnergy;
        }

        protected override bool ExecuteCore(Unit caster)
        {
            var target = caster.FindRandomAllyIncludingSelf(float.MaxValue);
            if (target == null)
                return false;

            // 攻击力加成与施法者当前血量相关
            float atkBoost = caster.Attributes.MaxHealth.finalValue * (Config.HealthToAtkPercent / 100f);

            target.AddBuff(new Units.Buffs.LifePowerBuff(
                atkBoost,                // 攻击力加成
                Config.BuffDuration,     // 持续时间
                caster,                  // 来源单位
                this                     // 来源技能
            ));
            return true;
        }

        public override string Description()
        {
            return DescriptionStatic();
        }
        public static string DescriptionStatic() => "随机为一个友方单位添加攻击力加成, 数值与施法者当前生命值相关.";

        public override string Name()
        {
            return NameStatic();
        }
        public static string NameStatic() => "生命之力";
    }
}