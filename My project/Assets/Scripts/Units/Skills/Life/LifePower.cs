using Model.Tetri;
using UnityEngine;

namespace Units.Skills
{
    public class LifePower : ActiveSkill
    {
        public override CellTypeId CellTypeId => CellTypeId.LifePower; // 如果有专属ID请替换
        public LifePowerConfig Config { get; }
        private Unit cachedTarget;

        public LifePower(LifePowerConfig config)
        {
            Config = config;
            RequiredEnergy = config.RequiredEnergy;
        }

        public override bool IsReady()
        {
            if (!base.IsReady())
                return false;
            cachedTarget = Owner.UnitManager.FindRandomAlly(
                self: Owner,
                range: float.MaxValue,
                includeSelf: true
            );
            if (cachedTarget == null)
                return false;

            return true;
        }


        protected override bool ExecuteCore()
        {
            if (cachedTarget == null)
                return false;
            // 攻击力加成与施法者当前血量相关
            float atkBoost = Owner.Attributes.MaxHealth.finalValue * (Config.HealthToAtkPercent / 100f);

            cachedTarget.AddBuff(new Units.Buffs.LifePowerBuff(
                atkBoost,                // 攻击力加成
                Config.BuffDuration,     // 持续时间
                Owner,                   // 来源单位
                this                     // 来源技能
            ));
            cachedTarget = null; // 用完清空
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