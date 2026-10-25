using System.Linq;
using Model;
using Presentation;
using Model.Resonance;
using Model.Skills;
using UnityEngine;

namespace Units.Skills
{
    /// <summary>
    /// IcyCage：对攻击范围内一个敌人施加Freeze（冻结）Buff，持续时间随冰系Cell数量提升
    /// </summary>
    public class IcyCage : ActiveSkill
    {
        public IcyCageLevelConfig Config { get; }
        private Unit targetEnemy;

        public IcyCage(IcyCageLevelConfig config)
        {
            Config = config;
            RequiredEnergy = config.RequiredEnergy;
        }

        protected override bool PrepareCastCore()
        {
            targetEnemy = null;
            // 使用与 AttackAction 相同的有效射程（含 AgentRadius）查找敌人
            if (!Context.TryGetClosestEnemyInAttackRange(out var found))
                return false;
            targetEnemy = found;

            return true;
        }

        protected override void ExecuteCore()
        {
            var config = (IcyCageSkillConfig)Definition.Config;
            var freezeBuff = new Buffs.Freeze(
                config.FreezeBuffDefinition,
                Context.GetResonanceCount(Definition.Resonance),
                new Buffs.BuffSource(Context.Unit, this)
            );
            targetEnemy.AddBuff(freezeBuff);
            targetEnemy = null;
        }

        public override string DescribeEffect()
        {
            var stats = CalcStats();
            return $"对攻击范围内一个敌人施加冻结效果：\n{StatBreakdownFormatter.Format(StatLabel.FreezeDuration, stats)}";
        }

        public static string DescriptionStatic() => "对攻击范围内一个敌人施加冻结效果";

        public override string Name()
        {
            return NameStatic();
        }
        public static string NameStatic() => "冰牢";
        private AdditiveValueBreakdown CalcStats()
        {
            return ((IcyCageSkillConfig)Definition.Config).FreezeBuffDefinition.CalculateStats(Context.GetResonanceCount(Definition.Resonance));
        }

    }
}