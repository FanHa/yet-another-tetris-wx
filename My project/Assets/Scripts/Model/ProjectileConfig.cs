using System.Collections.Generic;
using UnityEngine;

namespace Model
{
    [CreateAssetMenu(fileName = "ProjectileConfig", menuName = "Configs/ProjectileConfig")]
    public class ProjectileConfig : ScriptableObject
    {
        public GameObject BaseProjectilePrefab; // 投射物预制体
        public GameObject BombPrefab;
        public GameObject PrecisionArrowPrefab;
        public GameObject ChainLightningPrefab;
        public GameObject BloodBombPrefab;

        public GameObject FireballPrefab;

        public GameObject TempTargetPrefab;
    }
}