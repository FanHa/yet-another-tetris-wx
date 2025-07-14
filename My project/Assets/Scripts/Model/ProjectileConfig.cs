using System.Collections.Generic;
using UnityEngine;

namespace Model
{
    [CreateAssetMenu(fileName = "ProjectileConfig", menuName = "Configs/ProjectileConfig")]
    public class ProjectileConfig : ScriptableObject
    {
        public GameObject BaseProjectilePrefab; // 投射物预制体
        public GameObject MeleeProjectilePrefab;
        public GameObject BombPrefab;
        public GameObject PrecisionArrowPrefab;
        public GameObject ChainLightningPrefab;
        public GameObject BloodBombPrefab;

        public GameObject FireballPrefab;
        public GameObject SnowballPrefab;

        public GameObject TempTargetPrefab;

        public GameObject BlazingFieldPrefab;
        public GameObject FlameRingPrefab;

        public GameObject FrostZonePrefab;

        public GameObject IceShieldPrefab;
        public GameObject IcyCagePrefab;

        public GameObject WildWindPrefab;
    }
}