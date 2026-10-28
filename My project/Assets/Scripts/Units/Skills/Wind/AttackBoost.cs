using Model.Resonance;
using Model.Skills;
using UnityEngine;

namespace Units.Skills
{
    public class AttackBoost : ActiveSkill
    {
        private readonly AttackBoostSkillDefinition skillDefinition;

        public AttackBoost(AttackBoostSkillDefinition definition)
        {
            skillDefinition = definition;
            RequiredEnergy = definition.RequiredEnergy;
        }

        public override string DescribeEffect()
        {
            return
                Definition.Description + ":\n" +
                CreateBuff().DescribeAppliedEffect();
        }

        public override string Name() => Definition.DisplayName;

        protected override void ExecuteCore()
        {
            var buff = CreateBuff();

            var projectile = Object.Instantiate(
                skillDefinition.BuffProjectilePrefab,
                OwnerContext.Transform.position,
                UnityEngine.Quaternion.identity
            );
            projectile.Init(OwnerContext.Owner, OwnerContext.Owner, buff);
            projectile.Activate();
        }

        private Buffs.AttackBoostBuff CreateBuff()
        {
            return new Buffs.AttackBoostBuff(
                skillDefinition.AttackBoostBuffDefinition,
                OwnerContext.GetResonanceCount(Definition.Resonance),
                new Buffs.BuffSource(OwnerContext.Owner, this)
            );
        }
    }
}