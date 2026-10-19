using UnityEngine;
using UnityEngine.Serialization;
using Units.Projectiles;
using Model.Buffs;

namespace Model.Skills
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Skills/Configs/Ice/Snowball")]
    public class SnowballSkillConfig : SkillConfig<SnowballLevelConfig>
    {
        [Header("资源")]
        [SerializeField]
        private Snowball projectilePrefab;
        [SerializeField] private BuffDefinition chilledBuffDefinition;

        public Snowball ProjectilePrefab => projectilePrefab;
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
    public class SnowballLevelConfig : SkillLevelConfig, IRequiresEnergy
    {
        [Header("伤害")]
        public float BaseDamage = 10f;
        public float IceCellDamageBonus = 4f;

        [Header("Chilled效果")]
        public float BaseChilledDuration = 3f;
        public float ChilledDurationPerResonantCell = 0.5f;
        public int BaseChilledMoveSlowPercent = 10;
        public int ChilledMoveSlowPercentPerResonantCell = 3;
        public int BaseChilledAtkSlowPercent = 10;
        public int ChilledAtkSlowPercentPerResonantCell = 3;
        public int BaseChilledActionSlowPercent = 10;
        public int ChilledActionSlowPercentPerResonantCell = 3;
        public int BaseChilledEnergySlowPercent = 15;
        public int ChilledEnergySlowPercentPerResonantCell = 4;

        [Header("消耗")]
        public float RequiredEnergy = 40f;
        float IRequiresEnergy.RequiredEnergy => RequiredEnergy;
    }
}
