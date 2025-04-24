using UnityEngine;

[CreateAssetMenu(fileName = "VisualEffectConfig", menuName = "Configs/VisualEffectConfig")]
public class VisualEffectConfig : ScriptableObject
{
    [Header("Particle Effect Prefabs")]
    public GameObject WindStrikeAreaEffectPrefab;
}