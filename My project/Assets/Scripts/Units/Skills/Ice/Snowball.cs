using System.Linq;
using UnityEngine;

namespace Units.Skills
{
    /// <summary>
    /// SnowBall：对攻击范围内一个敌人发射雪球，造成冰属性伤害并施加Chilled（冰霜Debuff，可叠加或刷新）
    /// </summary>
    public class Snowball : Skill
    {
        public float BaseDamage = 10f;
        public float IceCellDamageBonus = 4f;
        public float BaseChilledDuration = 3f;
        public float ChilledDurationPerIceCell = 0.5f;
        public int BaseChilledMoveSlowPercent = 10;
        public int ChilledMoveSlowPercentPerIceCell = 3;
        public int BaseChilledAtkSlowPercent = 10;
        public int ChilledAtkSlowPercentPerIceCell = 3;
        public int BaseChilledEnergySlowPercent = 15;
        public int ChilledEnergySlowPercentPerIceCell = 4;

        private int iceCellCount = 0;

        public Snowball()
        {
            RequiredEnergy = 40f;
        }

        public void SetIceCellCount(int iceCellCount)
        {
            this.iceCellCount = iceCellCount;
        }

        protected override void ExecuteCore(Unit caster)
        {
            var enemiesInRange = FindEnemiesInRange(caster, caster.Attributes.AttackRange)
                .OrderBy(enemy => Vector3.Distance(caster.transform.position, enemy.transform.position))
                .ToList();

            if (enemiesInRange.Count == 0)
                return;

            Unit targetEnemy = enemiesInRange.First();

            // 实例化雪球投射物
            GameObject projectileInstance = Object.Instantiate(
                caster.ProjectileConfig.SnowballPrefab,
                caster.projectileSpawnPoint.position,
                Quaternion.identity
            );
            Units.Projectiles.Snowball snowBall = projectileInstance.GetComponent<Units.Projectiles.Snowball>();
            if (snowBall != null)
            {
                float totalDamage = BaseDamage + iceCellCount * IceCellDamageBonus;
                var damage = new Damages.Damage(totalDamage, Damages.DamageType.Ice);
                damage.SetSourceLabel(Name());
                damage.SetSourceUnit(caster);
                damage.SetTargetUnit(targetEnemy);

                // 计算Chilled参数
                float chilledDuration = BaseChilledDuration + iceCellCount * ChilledDurationPerIceCell;
                int moveSlowPercent = BaseChilledMoveSlowPercent + iceCellCount * ChilledMoveSlowPercentPerIceCell;
                int atkSlowPercent = BaseChilledAtkSlowPercent + iceCellCount * ChilledAtkSlowPercentPerIceCell;
                int energySlowPercent = BaseChilledEnergySlowPercent + iceCellCount * ChilledEnergySlowPercentPerIceCell;

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
            }
        }

        public override string Description()
        {
            return $"向攻击范围内一个敌人发射雪球，造成冰属性伤害并施加Chilled（冰霜Debuff，可叠加或刷新），所有效果随冰系Cell数量提升。";
        }

        public override string Name()
        {
            return "雪球";
        }
    }
}