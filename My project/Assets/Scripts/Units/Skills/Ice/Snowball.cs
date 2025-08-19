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
        private Unit cachedTarget;

        public Snowball(SnowballConfig config)
        {
            Config = config;
            RequiredEnergy = config.RequiredEnergy;
        }

        public override bool IsReady()
        {
            if (!base.IsReady())
                return false;

            // 找到攻击范围内的敌人
            cachedTarget = Owner.UnitManager.FindRandomEnemyInRange(Owner, Owner.Attributes.AttackRange);
            if (cachedTarget == null)
                return false;

            // 可以发射雪球
            return true;
        }

        protected override bool ExecuteCore()
        {
            var stats = CalcStats();
            // Todo 判断cachedTarget是否依然活跃

            // 实例化雪球投射物
            GameObject projectileInstance = Object.Instantiate(
                Owner.ProjectileConfig.SnowballPrefab,
                Owner.projectileSpawnPoint.position,
                Quaternion.identity
            );
            Units.Projectiles.Snowball snowBall = projectileInstance.GetComponent<Units.Projectiles.Snowball>();
            snowBall.Init(
                Owner,
                cachedTarget,
                stats.ChilledDuration.Final,
                (int)stats.MoveSlowPercent.Final,
                (int)stats.AtkSlowPercent.Final,
                (int)stats.EnergySlowPercent.Final,
                this
            );
            snowBall.Activate();
            // 清空目标
            cachedTarget = null;
            return true;
        }

        public override string Description()
        {
            var stats = CalcStats();
            return
                DescriptionStatic() + "\n" +
                $"{stats.ChilledDuration}\n" +
                $"{stats.MoveSlowPercent}\n" +
                $"{stats.AtkSlowPercent}\n" +
                $"{stats.EnergySlowPercent}";
        }
        public static string DescriptionStatic() => "向攻击范围内一个敌人发射雪球,施加减速Debuff.";

        public override string Name()
        {
            return NameStatic();
        }
        public static string NameStatic() => "雪球";
        
        private struct SnowballStats
        {
            public StatValue ChilledDuration;
            public StatValue MoveSlowPercent;
            public StatValue AtkSlowPercent;
            public StatValue EnergySlowPercent;
        }

        private SnowballStats CalcStats()
        {
            int iceCellCount = Owner != null && Owner.CellCounts.TryGetValue(AffinityType.Ice, out var count) ? count : 0;
            return new SnowballStats
            {
                ChilledDuration = new StatValue("减速效果持续时间", Config.BaseChilledDuration, iceCellCount * Config.ChilledDurationPerIceCell),
                MoveSlowPercent = new StatValue("移动速度降低(%)", Config.BaseChilledMoveSlowPercent, iceCellCount * Config.ChilledMoveSlowPercentPerIceCell),
                AtkSlowPercent = new StatValue("攻击速度降低(%)", Config.BaseChilledAtkSlowPercent, iceCellCount * Config.ChilledAtkSlowPercentPerIceCell),
                EnergySlowPercent = new StatValue("能量回复降低(%)", Config.BaseChilledEnergySlowPercent, iceCellCount * Config.ChilledEnergySlowPercentPerIceCell)
            };
        }
    }
}