using Model.Skills;
using UnityEngine;

namespace Units.Skills
{
    public class Fireball : ActiveSkill
    {
        private readonly FireballSkillDefinition skillDefinition;

        public Fireball(FireballSkillDefinition definition)
        {
            skillDefinition = definition;
            RequiredEnergy = definition.RequiredEnergy;
        }

        protected override void ExecuteCore()
        {
            Unit targetEnemy = UnitManager.FindClosestEnemy(Owner);
            if (targetEnemy == null)
            {
                return;
            }

            Units.Projectiles.Fireball fireball = Object.Instantiate(
                skillDefinition.ProjectilePrefab,
                Owner.projectileSpawnPoint.position,
                Quaternion.identity
            );

            fireball.Init(
                caster: Owner,
                target: targetEnemy,
                sourceSkill: this,
                resonantCellCount: Owner.GetResonanceCount(Definition.Resonance)
            );
            fireball.Activate();
        }

        public override string DescribeEffect()
        {
            return
                Definition.Description + ":\n" +
                skillDefinition.ProjectilePrefab.DescribeEffect(Owner.GetResonanceCount(Definition.Resonance));
        }

        public override string Name() => Definition.DisplayName;
    }
}