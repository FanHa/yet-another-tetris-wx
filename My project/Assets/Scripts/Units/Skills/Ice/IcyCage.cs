using System.Linq;
using Model.Tetri;
using UnityEngine;

namespace Units.Skills
{
    /// <summary>
    /// IcyCage：对攻击范围内一个敌人施加Freeze（冻结）Buff，持续时间随冰系Cell数量提升
    /// </summary>
    public class IcyCage : ActiveSkill
    {
        public override CellTypeId CellTypeId => CellTypeId.IcyCage;
        public IcyCageLevelConfig Config { get; }
        private Unit targetEnemy;

        public IcyCage(IcyCageLevelConfig config)
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
            targetEnemy = found;

            return true;
        }

        public override bool CanExecuteNow() =>
            targetEnemy != null && targetEnemy.IsActive;
        

        protected override bool ExecuteCore()
        {
            var stats = CalcStats();
            var freezeBuff = new Buffs.Freeze(
                stats.FreezeDuration.Final,
                Owner.SelfUnit,
                this
            );
            // todo 判断TargetEnemy 是否还活跃
            targetEnemy.AddBuff(freezeBuff);
            targetEnemy = null;
            return true;
        }

        public override string Description()
        {
            var stats = CalcStats();
            return $"对攻击范围内一个敌人施加冻结效果：\n{stats.FreezeDuration}";
        }

        public static string DescriptionStatic() => "对攻击范围内一个敌人施加冻结效果";

        public override string Name()
        {
            return NameStatic();
        }
        public static string NameStatic() => "冰牢";
        private struct IcyCageStats
        {
            public StatValue FreezeDuration;
        }

        private IcyCageStats CalcStats()
        {
            int iceCellCount = Owner != null && Owner.CellCounts.TryGetValue(AffinityType.Ice, out var count) ? count : 0;
            return new IcyCageStats
            {
                FreezeDuration = new StatValue("冻结持续时间", Config.BaseFreezeDuration, iceCellCount * Config.FreezeDurationPerIceCell)
            };
        }

    }
}