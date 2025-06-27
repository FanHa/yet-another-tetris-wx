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
    }
}