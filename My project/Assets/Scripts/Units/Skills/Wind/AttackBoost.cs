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
                Owner.transform.position,
                UnityEngine.Quaternion.identity
            );
            projectile.Init(Owner, Owner, buff);
            projectile.Activate();
        }

        private Buffs.AttackBoostBuff CreateBuff()
        {
            return new Buffs.AttackBoostBuff(
                skillDefinition.AttackBoostBuffDefinition,
                Owner.GetResonanceCount(Definition.Resonance),
                new Buffs.BuffSource(Owner, this)
            );
        }
    }
}