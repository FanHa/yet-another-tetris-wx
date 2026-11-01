using System.Linq;
using Model;
using Presentation;
using Model.Resonance;
using Model.Skills;
using UnityEngine;

namespace Units.Skills
{
    /// <summary>
    /// IcyCage：对最近的活动敌人施加Freeze（冻结）Buff，持续时间随冰系Cell数量提升
    /// </summary>
    public class IcyCage : ActiveSkill
    {
        private IcyCageSkillDefinition SkillDefinition => (IcyCageSkillDefinition)Definition;

        public IcyCage(IcyCageSkillDefinition definition)
        {
            RequiredEnergy = definition.RequiredEnergy;
        }

        protected override void ExecuteCore()
        {
            Unit targetEnemy = UnitManager.FindClosestEnemy(Owner);
            if (targetEnemy == null)
            {
                return;
            }

            targetEnemy.AddBuff(CreateFreezeBuff());
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