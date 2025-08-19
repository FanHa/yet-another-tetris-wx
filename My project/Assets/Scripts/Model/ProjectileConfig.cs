using System.Collections.Generic;
using UnityEngine;

namespace Model
{
    [CreateAssetMenu(fileName = "ProjectileConfig", menuName = "Configs/ProjectileConfig")]
    public class ProjectileConfig : ScriptableObject
    {
        [Header("通用")]
        public GameObject BuffProjectilePrefab; // Buff投射物预制体
        public GameObject BaseProjectilePrefab; // 投射物预制体
        public GameObject MeleeProjectilePrefab;
        public GameObject TemporaryTargetPrefab;

        [Header("技能投射物")]
        public GameObject ShadowArrowPrefab;
        public GameObject ChainLightningPrefab;

        public GameObject FireballPrefab;
        public GameObject SnowballPrefab;

        public GameObject BlazingFieldPrefab;
        public GameObject FlameRingPrefab;

        public GameObject FrostZonePrefab;

        public GameObject IceShieldPrefab;
        public GameObject IcyCagePrefab;

        public GameObject WildWindPrefab;
        public GameObject LifeBombPrefab;
    }
}