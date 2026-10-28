using Model;
using Presentation;
using Model.Tetri;
using Model.Skills;
using UnityEngine;

namespace Units.Skills
{
    public class EnergyCurse : Skill
    {
        private EnergyCurseSkillDefinition SkillDefinition => (EnergyCurseSkillDefinition)Definition;
        private Units.Projectiles.EnergyCurseZone ownedZone;

        public override string DescribeEffect()
        {
            int resonantCellCount = OwnerContext.GetResonanceCount(Definition.Resonance);
            var stats = SkillDefinition.CalculateStats(resonantCellCount);
            return
                Definition.Description + ":\n" +
                StatBreakdownFormatter.Format(StatLabel.DomainRadius, stats.Radius) + "\n" +
                StatBreakdownFormatter.Format(StatLabel.DomainDuration, stats.ZoneLifetime) + "\n" +
                SkillDefinition.ZonePrefab.DescribeEffect(resonantCellCount);
        }

        public override string Name() => Definition.DisplayName;

        public override void OnUnitDeath()
        {
            int resonantCellCount = OwnerContext.GetResonanceCount(Definition.Resonance);
            var stats = SkillDefinition.CalculateStats(resonantCellCount);

            ownedZone = Object.Instantiate(
                SkillDefinition.ZonePrefab,
                OwnerContext.Transform.position,
                Quaternion.identity
            );

            ownedZone.Initialize(
                caster: OwnerContext.Owner,
                radius: stats.Radius.FinalValue,
                duration: stats.ZoneLifetime.FinalValue,
                resonantCellCount: resonantCellCount,
                sourceSkill: this
            );
            ownedZone.Activate();
        }

        public override void OnRelease()
        {
            ClearOwnedZone();
        }

        private void ClearOwnedZone()
        {
            if (ownedZone == null)
                return;

            Units.Projectiles.EnergyCurseZone zone = ownedZone;
            ownedZone = null;
            zone.Deactivate();
            Object.Destroy(zone.gameObject);
        }
    }
}
