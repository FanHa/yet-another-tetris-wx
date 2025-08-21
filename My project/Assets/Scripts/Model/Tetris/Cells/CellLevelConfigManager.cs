using UnityEngine;
using Units.Skills;


namespace Model.Tetri
{
    [CreateAssetMenu(menuName = "SkillConfig/CellLevelConfigManager")]
    public class CellLevelConfigManager : ScriptableObject
    {
        [Header("火系技能")]
        public BlazingFieldConfigGroup BlazingFieldConfigGroup;
        public FireballConfigGroup FireballConfigGroup;
        public FlameInjectConfigGroup FlameInjectConfigGroup;
        public FlameRingConfigGroup FlameRingConfigGroup;

        [Header("冰系技能")]
        public SnowballConfigGroup SnowballConfigGroup;
        public IcyCageConfigGroup IcyCageConfigGroup;
        public IceShieldConfigGroup IceShieldConfigGroup;
        public FrostZoneConfigGroup FrostZoneConfigGroup;

        [Header("风系技能")]
        public WindShiftConfigGroup WindShiftConfigGroup;
        public WildWindConfigGroup WildWindConfigGroup;
        public AttackBoostConfigGroup AttackBoostConfigGroup;
        public HitAndRunConfigGroup HitAndRunConfigGroup; // HitAndRun 没有配置

        [Header("生命")]
        public LifeBombConfigGroup LifeBombConfigGroup;
        public LifeShieldConfigGroup LifeShieldConfigGroup;
        public LifePowerConfigGroup LifePowerConfigGroup;
        public LifeEchoConfigGroup LifeEchoConfigGroup;

        [Header("暗影")]
        public ShadowAttackConfigGroup ShadowAttackConfigGroup;
        public ShadowStepConfigGroup ShadowStepConfigGroup;
        public ShadowArrowConfigGroup ShadowArrowConfigGroup;

        [Header("迅捷")]
        public ChargeConfigGroup ChargeConfigGroup;
    }
}