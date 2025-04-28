using System.Collections.Generic;
using UnityEngine;

namespace Model
{
    [CreateAssetMenu(fileName = "ProjectileConfig", menuName = "Configs/ProjectileConfig")]
    public class ProjectileConfig : ScriptableObject
    {
        public GameObject BaseProjectilePrefab; // 投射物预制体
        public GameObject BombPrefab; // TODO 暂时所有projectile的prefab都放到这里,以后再改
        public GameObject PrecisionArrowPrefab;
        public GameObject ChainLightningPrefab;
        public GameObject BloodBombPrefab;

        public GameObject TempTargetPrefab;
    }
}