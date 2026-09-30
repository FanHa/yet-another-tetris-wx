using UnityEngine;
using Units.Projectiles;

namespace Model.Skills
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Skills/Configs/Wind/Attack Boost")]
    public class AttackBoostSkillConfig : SkillConfig<AttackBoostLevelConfig>
    {
        [Header("资源")]
        [SerializeField]
        private BuffProjectile buffProjectilePrefab;

        public BuffProjectile BuffProjectilePrefab => buffProjectilePrefab;
    }
    [System.Serializable]
    public class AttackBoostLevelConfig : SkillLevelConfig
    {
        [Header("通用")]
        public float RequiredEnergy;
        public float Duration;

        [Header("属性")]
        public float AtkSpeedPercent;

        [Header("加成")]
        public float AtkSpeedAdditionPercentPerWindCell; // 每个风系方块增加的攻击速度百分比
    }
}