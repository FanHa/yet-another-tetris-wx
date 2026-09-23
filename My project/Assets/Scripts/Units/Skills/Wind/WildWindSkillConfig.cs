using UnityEngine;

namespace Units.Skills
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Skills/Configs/Wind/Wild Wind")]
    public class WildWindSkillConfig : SkillConfig<WildWindLevelConfig>
    {
    }

    [System.Serializable]
    public class WildWindLevelConfig : SkillLevelConfig
    {
        [Header("技能消耗")]
        public float RequiredEnergy;

        [Header("基础属性")]
        public float Damage;
        public float DebuffDuration;
        public int MoveSlowPercent;
        public int AtkReducePercent;
        public float Radius;
        public float Duration;

        [Header("加成")]
        public float DamageAdditionPerWindCell = 3f; // 每个风系方块增加的伤害
    }
}