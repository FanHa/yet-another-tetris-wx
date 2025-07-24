using System.Linq;
using Model.Tetri;
using UnityEngine;

namespace Units.Skills
{
    public class LifeShield : ActiveSkill
    {
        public override CellTypeId CellTypeId => CellTypeId.LifeShield;
        public LifeShieldConfig Config { get; }

        public LifeShield(LifeShieldConfig config)
        {
            Config = config;
            RequiredEnergy = config.RequiredEnergy;
        }

        public override bool IsReady()
        {
            // todo 重写,这里需要判定自身血量条件
            return base.IsReady();
        }

        protected override bool ExecuteCore(Unit caster)
        {
            // 找到所有友方单位（不包括自己）
            var target = caster.UnitManager.FindRandomAlly(
                self: caster,
                range: float.MaxValue,
                includeSelf: false
            );

            // todo 尽量不要重复
            if (target == null)
                return false;

            float shieldAmount = caster.Attributes.CurrentHealth * (Config.LifeCostPercent / 100f);
            // todo 扣除自己血量

            var buff = new Units.Buffs.LifeShieldBuff(
                shieldAmount,         // 护盾值
                Config.BuffDuration,  // 持续时间
                caster,               // 来源单位
                this                  // 来源技能
            );
            var prefab = caster.ProjectileConfig.BuffProjectilePrefab;
            var projectileObj = Object.Instantiate(prefab, caster.transform.position, Quaternion.identity);
            var projectile = projectileObj.GetComponent<Units.Projectiles.BuffProjectile>();
            projectile.Init(caster, target, buff);
            projectile.Activate();
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