using UnityEngine;
using Units.Skills;

namespace Model.Tetri
{
    [CreateAssetMenu(menuName = "ConfigRegistry/CellSkillConfigRegistry")]
    public class CellSkillConfigRegistry : ScriptableObject
    {
        [Header("火系技能")]
        public BlazingFieldSkillConfig BlazingFieldSkillConfig;
        public FireballSkillConfig FireballSkillConfig;
        public FlameInjectSkillConfig FlameInjectSkillConfig;
        public FlameRingSkillConfig FlameRingSkillConfig;

        [Header("冰系技能")]
        public SnowballSkillConfig SnowballSkillConfig;
        public IcyCageSkillConfig IcyCageSkillConfig;
        public IceShieldSkillConfig IceShieldSkillConfig;
        public FrostZoneSkillConfig FrostZoneSkillConfig;
        public IceBreakerSkillConfig IceBreakerSkillConfig;

        [Header("风系技能")]
        public WindShiftSkillConfig WindShiftSkillConfig;
        public WildWindSkillConfig WildWindSkillConfig;
        public AttackBoostSkillConfig AttackBoostSkillConfig;
        public WindKnockbackSkillConfig WindKnockbackSkillConfig;

        [Header("生命")]
        public LifeBombSkillConfig LifeBombSkillConfig;
        public LifeShieldSkillConfig LifeShieldSkillConfig;
        public LifePowerSkillConfig LifePowerSkillConfig;
        public LifeEchoSkillConfig LifeEchoSkillConfig;
        public GuardAllySkillConfig GuardAllySkillConfig;

        [Header("暗影")]
        public ShadowAttackSkillConfig ShadowAttackSkillConfig;
        public ShadowStepSkillConfig ShadowStepSkillConfig;
        public ShadowArrowSkillConfig ShadowArrowSkillConfig;
        public VulnerabilityFieldSkillConfig VulnerabilityFieldSkillConfig;

        [Header("迅捷")]
        public ChargeSkillConfig ChargeSkillConfig;

        [Header("吸取")]
        public EnergyAbsorbSkillConfig EnergyAbsorbSkillConfig;

        [Header("电系")]
        public ThunderStrikeSkillConfig ThunderStrikeSkillConfig;
    }
}