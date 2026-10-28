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
            int resonantCellCount = Context.GetResonanceCount(Definition.Resonance);
            var stats = SkillDefinition.CalculateStats(resonantCellCount);
            return
                DescriptionStatic() + ":\n" +
                StatBreakdownFormatter.Format(StatLabel.DomainRadius, stats.Radius) + "\n" +
                StatBreakdownFormatter.Format(StatLabel.DomainDuration, stats.ZoneLifetime) + "\n" +
                SkillDefinition.ZonePrefab.DescribeEffect(resonantCellCount);
        }

        public static string DescriptionStatic() => "死亡时在当前位置生成诅咒区域，降低区域内敌人的能量回复速度。";

        public override string Name() => NameStatic();

        public static string NameStatic() => "能量诅咒";

        public override void OnUnitDeath()
        {
            int resonantCellCount = Context.GetResonanceCount(Definition.Resonance);
            var stats = SkillDefinition.CalculateStats(resonantCellCount);

            ownedZone = Object.Instantiate(
                SkillDefinition.ZonePrefab,
                Context.Transform.position,
                Quaternion.identity
            );

            ownedZone.Initialize(
                caster: Context.Unit,
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
