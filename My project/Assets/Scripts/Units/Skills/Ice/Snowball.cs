using System.Linq;
using Model.Resonance;
using Model.Skills;
using UnityEngine;

namespace Units.Skills
{
    public class Snowball : ActiveSkill
    {
        public SnowballLevelConfig Config { get; }
        private Unit cachedTarget;

        private SnowballSkillConfig SkillConfig => (SnowballSkillConfig)Definition.Config;

        public Snowball(SnowballLevelConfig config)
        {
            Config = config;
            RequiredEnergy = config.RequiredEnergy;
        }

        protected override bool PrepareCastCore()
        {
            cachedTarget = null;
            // 使用与 AttackAction 相同的有效射程（含 AgentRadius）查找敌人
            if (!Owner.TryGetClosestEnemyInAttackRange(out var found))
                return false;
            cachedTarget = found;

            // 可以发射雪球
            return true;
        }

        protected override void ExecuteCore()
        {
            var stats = CalcStats();

            // 实例化雪球投射物
            Units.Projectiles.Snowball snowBall = Object.Instantiate(
                SkillConfig.ProjectilePrefab,
                Owner.projectileSpawnPoint.position,
                Quaternion.identity
            );
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
            int resonantCellCount = Owner != null ? Owner.GetResonanceCount(Definition.Resonance) : 0;
            return new SnowballStats
            {
                ChilledDuration = new StatValue("减速效果持续时间", Config.BaseChilledDuration, resonantCellCount * Config.ChilledDurationPerResonantCell),
                MoveSlowPercent = new StatValue("移动速度降低(%)", Config.BaseChilledMoveSlowPercent, resonantCellCount * Config.ChilledMoveSlowPercentPerResonantCell),
                AtkSlowPercent = new StatValue("攻击速度降低(%)", Config.BaseChilledAtkSlowPercent, resonantCellCount * Config.ChilledAtkSlowPercentPerResonantCell),
                ActionSlowPercent = new StatValue("动作速率降低(%)", Config.BaseChilledActionSlowPercent, resonantCellCount * Config.ChilledActionSlowPercentPerResonantCell),
                EnergySlowPercent = new StatValue("能量回复降低(%)", Config.BaseChilledEnergySlowPercent, resonantCellCount * Config.ChilledEnergySlowPercentPerResonantCell)
            };
        }
    }
}