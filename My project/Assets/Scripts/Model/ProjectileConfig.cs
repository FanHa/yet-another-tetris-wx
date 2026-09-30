using System.Collections.Generic;
using UnityEngine;

namespace Model
{
    [CreateAssetMenu(fileName = "ProjectileConfig", menuName = "Game Data/Shared/Projectiles/Projectile Config")]
    public class ProjectileConfig : ScriptableObject
    {
        [Header("通用")]
        public GameObject BuffProjectilePrefab; // Buff投射物预制体
        public GameObject RangeAttackProjectilePrefab; // 投射物预制体
        public GameObject TemporaryTargetPrefab;

    }
}