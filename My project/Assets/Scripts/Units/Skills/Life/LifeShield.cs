using System.Linq;
using Model.Tetri;
using UnityEngine;

namespace Units.Skills
{
    public class LifeShield : ActiveSkill
    {
        public override CellTypeId CellTypeId => CellTypeId.LifeShield;
        public LifeShieldConfig Config { get; }
        private Unit cachedTarget;

        public LifeShield(LifeShieldConfig config)
        {
            Config = config;
            RequiredEnergy = config.RequiredEnergy;
        }

        public override bool IsReady()
        {
            if (!base.IsReady())
                return false;

            // 找到友方目标（不包括自己）
            cachedTarget = Owner.UnitManager.FindRandomAlly(
                self: Owner,
                range: float.MaxValue,
                includeSelf: false
            );
            if (cachedTarget == null)
                return false;

            return true;
        }


        protected override bool ExecuteCore()
        {
            if (cachedTarget == null)
                return false;

            float shieldAmount = Owner.Attributes.CurrentHealth * (Config.LifeCostPercent / 100f);

            var buff = new Units.Buffs.LifeShieldBuff(
                shieldAmount,         // 护盾值
                Config.BuffDuration,  // 持续时间
                Owner,               // 来源单位
                this                  // 来源技能
            );
            var prefab = Owner.ProjectileConfig.BuffProjectilePrefab;
            var projectileObj = Object.Instantiate(prefab, Owner.transform.position, Quaternion.identity);
            var projectile = projectileObj.GetComponent<Units.Projectiles.BuffProjectile>();
            projectile.Init(Owner, cachedTarget, buff);
            projectile.Activate();

            var damage = new Damages.Damage(shieldAmount, Damages.DamageType.Skill);
            damage.SetSourceUnit(Owner);
            damage.SetTargetUnit(Owner);
            damage.SetSourceLabel(Name());
            Owner.TakeDamage(damage);

            cachedTarget = null;
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