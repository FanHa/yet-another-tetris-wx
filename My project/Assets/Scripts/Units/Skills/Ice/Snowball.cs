using System.Linq;
using Model.Tetri;
using UnityEngine;

namespace Units.Skills
{
    /// <summary>
    /// SnowBall：对攻击范围内一个敌人发射雪球，造成冰属性伤害并施加Chilled（冰霜Debuff，可叠加或刷新）
    /// </summary>
    public class Snowball : ActiveSkill
    {
        public override CellTypeId CellTypeId => CellTypeId.Snowball;
        public SnowballConfig Config { get; }

        public Snowball(SnowballConfig config)
        {
            Config = config;
            RequiredEnergy = config.RequiredEnergy;
        }

        protected override bool ExecuteCore(Unit caster)
        {
            var enemiesInRange = caster.FindEnemiesInRange(caster.Attributes.AttackRange)
                .OrderBy(enemy => Vector3.Distance(caster.transform.position, enemy.transform.position))
                .ToList();

            if (enemiesInRange.Count == 0)
                return false;

            Unit targetEnemy = enemiesInRange.First();

            // 实例化雪球投射物
            GameObject projectileInstance = Object.Instantiate(
                caster.ProjectileConfig.SnowballPrefab,
                caster.projectileSpawnPoint.position,
                Quaternion.identity
            );
            Units.Projectiles.Snowball snowBall = projectileInstance.GetComponent<Units.Projectiles.Snowball>();
            int iceCellCount = caster.CellCounts.TryGetValue(AffinityType.Ice, out var count) ? count : 0;
            float totalDamage = Config.BaseDamage + iceCellCount * Config.IceCellDamageBonus;
            var damage = new Damages.Damage(totalDamage, Damages.DamageType.Skill);
            damage.SetSourceLabel(Name());
            damage.SetSourceUnit(caster);
            damage.SetTargetUnit(targetEnemy);
            // 计算Chilled参数
            float chilledDuration = Config.BaseChilledDuration + iceCellCount * Config.ChilledDurationPerIceCell;
            int moveSlowPercent = Config.BaseChilledMoveSlowPercent + iceCellCount * Config.ChilledMoveSlowPercentPerIceCell;
            int atkSlowPercent = Config.BaseChilledAtkSlowPercent + iceCellCount * Config.ChilledAtkSlowPercentPerIceCell;
            int energySlowPercent = Config.BaseChilledEnergySlowPercent + iceCellCount * Config.ChilledEnergySlowPercentPerIceCell;
            var chilled = new Units.Buffs.Chilled(
                chilledDuration,
                moveSlowPercent,
                atkSlowPercent,
                energySlowPercent,
                caster,
                this
            );
            snowBall.SetChilled(chilled);
            snowBall.Init(caster, targetEnemy.transform, damage);
            snowBall.Activate();
            return true;
            
        }

        public override string Description()
        {
            return DescriptionStatic();
        }
        public static string DescriptionStatic() => "向攻击范围内一个敌人发射雪球,造成伤害并施加减速Debuff.";

        public override string Name()
        {
            return NameStatic();
        }
        public static string NameStatic() => "雪球";
    }
}