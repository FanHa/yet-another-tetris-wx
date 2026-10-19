using UnityEngine;
using UnityEngine.Serialization;
using Units.Projectiles;
using Model.Buffs;

namespace Model.Skills
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Skills/Configs/Ice/Frost Zone")]
    public class FrostZoneSkillConfig : SkillConfig<FrostZoneLevelConfig>
    {
        [Header("资源")]
        [SerializeField]
        private FrostZone projectilePrefab;
        [SerializeField] private BuffDefinition chilledBuffDefinition;

        public FrostZone ProjectilePrefab => projectilePrefab;
        public BuffDefinition ChilledBuffDefinition => chilledBuffDefinition;

    #if UNITY_EDITOR
        protected override void ValidateSpecificConfig()
        {
            ValidateRequiredReference(projectilePrefab, nameof(projectilePrefab));
            ValidateRequiredReference(chilledBuffDefinition, nameof(chilledBuffDefinition));
        }
    #endif
    }

    [System.Serializable]
    public class FrostZoneLevelConfig : SkillLevelConfig, IRequiresEnergy
    {
        [Header("基础属性")]
        [Tooltip("基础半径")]
        public float BaseRadius = 1.5f;
        [Tooltip("基础持续时间")]
        public float BaseDuration = 5f;
        [Tooltip("基础伤害")]
        public float BaseDamage = 8f;
        [Tooltip("基础冰冻时长")]
        public float BaseChilledDuration = 2f;
        [Tooltip("基础移动减速百分比")]
        public int BaseChilledMoveSlowPercent = 10;

        [Header("匹配共鸣 Cell 加成")]
        [Tooltip("匹配共鸣 Cell 增加的半径")]
        public float RadiusPerResonantCell = 0.2f;
        [Tooltip("匹配共鸣 Cell 增加的持续时间")]
        public float DurationPerResonantCell = 1f;
        [Tooltip("匹配共鸣 Cell 增加的伤害")]
        public float DamagePerResonantCell = 3f;
        [Tooltip("匹配共鸣 Cell 增加的减速时长")]
        public float ChilledDurationPerResonantCell = 0.5f;

        [Header("减速效果")]
        [Tooltip("匹配共鸣 Cell 增加的移动减速百分比")]
        public int ChilledMoveSlowPercentPerResonantCell = 3;
        [Tooltip("基础攻击减速百分比")]
        public int BaseChilledAtkSlowPercent = 10;
        [Tooltip("匹配共鸣 Cell 增加的攻击减速百分比")]
        public int ChilledAtkSlowPercentPerResonantCell = 3;
        [Tooltip("基础动作减速百分比")]
        public int BaseChilledActionSlowPercent = 10;
        [Tooltip("匹配共鸣 Cell 增加的动作减速百分比")]
        public int ChilledActionSlowPercentPerResonantCell = 3;
        [Tooltip("基础能量回复减速百分比")]
        public int BaseChilledEnergySlowPercent = 15;
        [Tooltip("匹配共鸣 Cell 增加的能量回复减速百分比")]
        public int ChilledEnergySlowPercentPerResonantCell = 4;

        [Header("消耗")]
        [Tooltip("释放所需能量")]
        public float RequiredEnergy = 120f;
        float IRequiresEnergy.RequiredEnergy => RequiredEnergy;
    }
}