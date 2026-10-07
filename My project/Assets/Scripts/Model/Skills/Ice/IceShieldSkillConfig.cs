using UnityEngine;
using Units.Projectiles;
using Model.Buffs;

namespace Model.Skills
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Skills/Configs/Ice/Ice Shield")]
    public class IceShieldSkillConfig : SkillConfig<IceShieldLevelConfig>
    {
        [Header("资源")]
        [SerializeField]
        private IceShield projectilePrefab;
        [SerializeField] private BuffDefinition iceShieldBuffDefinition;
        [SerializeField] private BuffDefinition chilledBuffDefinition;

        public IceShield ProjectilePrefab => projectilePrefab;
        public BuffDefinition IceShieldBuffDefinition => iceShieldBuffDefinition;
        public BuffDefinition ChilledBuffDefinition => chilledBuffDefinition;

    #if UNITY_EDITOR
        protected override void ValidateSpecificConfig()
        {
            ValidateRequiredReference(projectilePrefab, nameof(projectilePrefab));
            ValidateRequiredReference(iceShieldBuffDefinition, nameof(iceShieldBuffDefinition));
            ValidateRequiredReference(chilledBuffDefinition, nameof(chilledBuffDefinition));
        }
    #endif
    }

    [System.Serializable]
    public class IceShieldLevelConfig : SkillLevelConfig
    {
        [Header("减速效果")]
        public int BaseMoveSlowPercent = 10;
        public int MoveSlowPercentPerIceCell = 5;
        public int BaseAtkSlowPercent = 10;
        public int AtkSlowPercentPerIceCell = 5;
        public int BaseActionSlowPercent = 10;
        public int ActionSlowPercentPerIceCell = 5;
        public int BaseEnergySlowPercent = 15;
        public int EnergySlowPercentPerIceCell = 5;

        [Header("护盾与冰冻")]
        public float BuffDuration = -1f;
        public float BaseChilledDuration = 5f;
        public float ChilledDurationAdditionPerIceCell = 0.5f;
    }
}