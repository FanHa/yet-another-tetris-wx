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
        private IcyCageSkillDefinition SkillDefinition => (IcyCageSkillDefinition)Definition;
        private Unit targetEnemy;

        public IcyCage(IcyCageSkillDefinition definition)
        {
            RequiredEnergy = definition.RequiredEnergy;
        }

        protected override bool PrepareCastCore()
        {
            targetEnemy = null;
            // 使用与 AttackAction 相同的有效射程（含 AgentRadius）查找敌人
            if (!Owner.TryGetClosestEnemyInAttackRange(out var found))
                return false;
            targetEnemy = found;

            return true;
        }

        protected override void ExecuteCore()
        {
            targetEnemy.AddBuff(CreateFreezeBuff());
            targetEnemy = null;
        }

        public override string DescribeEffect()
        {
            return Definition.Description + ":\n" + CreateFreezeBuff().DescribeAppliedEffect();
        }

        public override string Name() => Definition.DisplayName;

        private Buffs.Freeze CreateFreezeBuff() => new(
            SkillDefinition.FreezeBuffDefinition,
            Owner.GetResonanceCount(Definition.Resonance),
            new Buffs.BuffSource(Owner, this));

    }
}