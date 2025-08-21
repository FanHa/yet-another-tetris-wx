using Model.Tetri;
using UnityEngine;

namespace Units.Skills
{
    public class Charge : ActiveSkill
    {
        public override CellTypeId CellTypeId => CellTypeId.Charge;
        public ChargeConfig Config { get; }

        public Charge(ChargeConfig config)
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

        protected override bool ExecuteCore()
        {
            var stats = CalcStats();

            // 找到距离自己最远的敌人
            Unit targetEnemy = Owner.UnitManager.FindFurthestEnemy(Owner);
            if (targetEnemy == null)
                return false;

            var prefab = Owner.ProjectileConfig.ChargePrefab;
            var chargeObj = Object.Instantiate(prefab, Owner.transform.position, Quaternion.identity);
            var chargeProjectile = chargeObj.GetComponent<Units.Projectiles.Charge>();
            chargeProjectile.Init(
                owner: Owner,
                target: targetEnemy,
                speed: Owner.Attributes.MoveSpeed.finalValue,
                chargeDamage: stats.ChargeDamage.Final,
                sourceSkill: this
            );
            chargeProjectile.Activate();
            return true;
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