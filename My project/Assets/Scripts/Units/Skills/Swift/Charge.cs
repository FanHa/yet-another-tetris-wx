using Model.Tetri;
using Model.Skills;
using UnityEngine;

namespace Units.Skills
{
    public class Charge : ActiveSkill
    {
        public ChargeLevelConfig Config { get; }

        private ChargeSkillConfig SkillConfig => (ChargeSkillConfig)Definition.Config;

        public Charge(ChargeLevelConfig config)
        {
            Config = config;
            RequiredEnergy = config.RequiredEnergy;
        }

        private struct ChargeStats
        {
            public StatValue ChargeDamage;
        }


        private ChargeStats CalcStats()
        {
            float moveSpeed = Owner.Attributes.MoveSpeed.finalValue;
            float chargeDamage = Config.ChargeDamage + moveSpeed * Config.DamageBonusPerSpeed;
            return new ChargeStats
            {
                ChargeDamage = new StatValue("冲刺伤害", chargeDamage),
            };
        }

        protected override void ExecuteCore()
        {
            var stats = CalcStats();

            // 找到距离自己最远的敌人
            Unit targetEnemy = Owner.FindFurthestEnemy();
            if (targetEnemy == null)
                return;

            var chargeProjectile = Object.Instantiate(
                SkillConfig.ProjectilePrefab,
                Owner.transform.position,
                Quaternion.identity
            );
            chargeProjectile.Init(
                owner: Owner.SelfUnit,
                target: targetEnemy,
                speed: 2f * Owner.Attributes.MoveSpeed.finalValue,
                chargeDamage: stats.ChargeDamage.Final,
                avoidancePriority: Config.AvoidancePriority,
                sourceSkill: this
            );
            chargeProjectile.Activate();
        }

        public override string Description()
        {
            var stats = CalcStats();
            return
                DescriptionStatic() +
                $"{stats.ChargeDamage}\n";
        }

        public override string Name() => NameStatic();

        public static string DescriptionStatic() => "冲向距离自己最远的敌人，沿途对敌人造成伤害";

        public static string NameStatic() => "冲锋";
    }
}