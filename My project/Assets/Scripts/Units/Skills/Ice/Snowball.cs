using System.Linq;
using Model.Tetri;
using UnityEngine;

namespace Units.Skills
{
    public class Snowball : ActiveSkill
    {
        public override CellTypeId CellTypeId => CellTypeId.Snowball;
        public SnowballLevelConfig Config { get; }
        private Unit cachedTarget;

        public Snowball(SnowballLevelConfig config)
        {
            Config = config;
            RequiredEnergy = config.RequiredEnergy;
        }

        public override bool IsReady()
        {
            if (!base.IsReady())
                return false;

            // 使用与 AttackAction 相同的有效射程（含 AgentRadius）查找敌人
            if (!Owner.TryGetClosestEnemyInAttackRange(out var found))
                return false;
            cachedTarget = found;

            // 可以发射雪球
            return true;
        }

        public override bool CanExecuteNow() => 
            cachedTarget != null && cachedTarget.IsActive;

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
                Owner.SelfUnit,
                cachedTarget,
                stats.ChilledDuration.Final,
                (int)stats.MoveSlowPercent.Final,
                (int)stats.AtkSlowPercent.Final,
                (int)stats.ActionSlowPercent.Final,
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
                $"{stats.ActionSlowPercent}\n" +
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
            public StatValue ActionSlowPercent;
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
                ActionSlowPercent = new StatValue("动作速率降低(%)", Config.BaseChilledActionSlowPercent, iceCellCount * Config.ChilledActionSlowPercentPerIceCell),
                EnergySlowPercent = new StatValue("能量回复降低(%)", Config.BaseChilledEnergySlowPercent, iceCellCount * Config.ChilledEnergySlowPercentPerIceCell)
            };
        }
    }
}