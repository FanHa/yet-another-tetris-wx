using Model.Tetri;
using Model.Skills;
using UnityEngine;

namespace Units.Skills
{
    public class LifePower : ActiveSkill
    {
        public LifePowerLevelConfig Config { get; }
        private Unit cachedTarget;

        public LifePower(LifePowerLevelConfig config)
        {
            Config = config;
            RequiredEnergy = config.RequiredEnergy;
        }

        private struct LifePowerStats
        {
            public StatValue HealthToAtkPercent;
            public StatValue BuffDuration;
        }

        private LifePowerStats CalcStats()
        {
            return new LifePowerStats
            {
                HealthToAtkPercent = new StatValue("生命转换率(%)", Config.HealthToAtkPercent),
                BuffDuration = new StatValue("Buff时间", Config.BuffDuration)
            };
        }


        protected override bool PrepareCastCore()
        {
            cachedTarget = null;
            cachedTarget = Owner.FindRandomAlly(
                range: float.MaxValue,
                includeSelf: true
            );
            if (cachedTarget == null)
                return false;

            return true;
        }

        protected override void ExecuteCore()
        {
            if (cachedTarget == null)
            return;

            var stats = CalcStats();
            // 攻击力加成与施法者最大生命值相关
            float atkBoost = Owner.Attributes.MaxHealth.finalValue * (stats.HealthToAtkPercent.Final / 100f);

            cachedTarget.AddBuff(new Units.Buffs.LifePowerBuff(
                atkBoost,
                stats.BuffDuration.Final,
                Owner.SelfUnit,
                this
            ));
            cachedTarget = null;
        }

        public override string Description()
        {
            var stats = CalcStats();
            return
                DescriptionStatic() + "\n" +
                $"{stats.HealthToAtkPercent}" +
                $"{stats.BuffDuration}";
        }

        public static string DescriptionStatic() => "转换自身生命值为一个友方单位添加攻击力Buff";

        public override string Name()
        {
            return NameStatic();
        }
        public static string NameStatic() => "生命之力";
    }
}