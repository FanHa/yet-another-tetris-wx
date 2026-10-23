using Model;
using Model.Skills;
using UnityEngine;

namespace Units.Skills
{
    public class FlameRing : Skill
    {
        public FlameRingLevelConfig Config { get; }
        private Units.Projectiles.FlameRing areaEffect;

        public FlameRing(FlameRingLevelConfig config)
        {
            Config = config;
        }

        private struct FlameRingStats
        {
            public StatBreakdownDisplay Radius;
            public StatBreakdownDisplay AreaDuration;
        }

        private int GetResonantCellCount()
        {
            return Owner != null ? Owner.GetResonanceCount(Definition.Resonance) : 0;
        }

        private FlameRingStats CalcStats()
        {
            int resonantCellCount = GetResonantCellCount();
            return new FlameRingStats
            {
                Radius = new StatBreakdownDisplay(StatLabel.ActionRadius, Config.BaseRadius, resonantCellCount * Config.RadiusPerResonantCell),
                AreaDuration = new StatBreakdownDisplay(StatLabel.AreaDuration, Config.AreaDuration)
            };
        }

        public override string Description()
        {
            var stats = CalcStats();
            return
                DescriptionStatic() + ":\n" +
                ((FlameRingSkillConfig)Definition.Config).ProjectilePrefab.DescribeEffect(GetResonantCellCount()) + "\n" +
                $"{stats.Radius}\n";
        }

        public static string DescriptionStatic() => "对周围一圈敌人持续造成火焰伤害";

        public override string Name()
        {
            return NameStatic();
        }
        public static string NameStatic() => "火环";

        public override void OnActivate()
        {
            var stats = CalcStats();
            var skillConfig = (FlameRingSkillConfig)Definition.Config;
            areaEffect = Object.Instantiate(
                skillConfig.ProjectilePrefab,
                Owner.transform.position,
                Quaternion.identity
            );
            areaEffect.Initialize(
                owner: Owner.SelfUnit,
                radius: stats.Radius.FinalValue,
                sourceSkill: this,
                duration: stats.AreaDuration.FinalValue,
                resonantCellCount: GetResonantCellCount()
            );
            areaEffect.Activate();
        }

        public override void OnDeactivate()
        {
            ClearAreaEffect();
        }

        public override void OnRelease()
        {
            ClearAreaEffect();
        }

        private void ClearAreaEffect()
        {
            if (areaEffect == null)
            {
                return;
            }

            Object.Destroy(areaEffect.gameObject);
            areaEffect = null;
        }
    }
}