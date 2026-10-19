using System.Linq;
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
            public StatValue DotDps;
            public StatValue DotDuration;
            public StatValue Radius;
            public StatValue AreaDuration;
        }

        private FlameRingStats CalcStats()
        {
            int resonantCellCount = Owner != null ? Owner.GetResonanceCount(Definition.Resonance) : 0;
            return new FlameRingStats
            {
                DotDps = new StatValue("每秒火焰伤害", Config.BaseDotDps, resonantCellCount * Config.DotDpsPerResonantCell),
                DotDuration = new StatValue("火焰伤害持续时间", Config.BaseDotDuration, resonantCellCount * Config.DotDurationPerResonantCell),
                Radius = new StatValue("作用半径", Config.BaseRadius, resonantCellCount * Config.RadiusPerResonantCell),
                AreaDuration = new StatValue("区域持续时间", Config.AreaDuration)
            };
        }

        public override string Description()
        {
            var stats = CalcStats();
            return
                DescriptionStatic() + ":\n" +
                $"{stats.DotDps}\n" +
                $"{stats.DotDuration}\n" +
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
                radius: stats.Radius.Final,
                sourceSkill: this,
                burnDefinition: skillConfig.BurnBuffDefinition,
                duration: stats.AreaDuration.Final,
                dotDps: stats.DotDps.Final,
                dotDuration: stats.DotDuration.Final
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