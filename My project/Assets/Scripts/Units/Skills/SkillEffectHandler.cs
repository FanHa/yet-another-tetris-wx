using System;
using System.Collections.Generic;
using UnityEngine;

namespace Units.Skills
{
    public class SkillEffectHandler : MonoBehaviour
    {
        [SerializeField] private GameObject blazingFieldPrefab;
        [SerializeField] private GameObject flamingRingPrefab;
        [SerializeField] private GameObject iceShieldPrefab;
        [SerializeField] private GameObject frostZonePrefab;
        [SerializeField] private GameObject icyCagePrefab;


        internal void HandleSkillEffect(SkillEffectContext context)
        {
            if (context == null) return;
            GameObject prefab;
            if (context.Skill is BlazingField)
            {
                prefab = blazingFieldPrefab;
                Vector3 spawnPos = context.Position;
                GameObject instance = Instantiate(prefab, spawnPos, Quaternion.identity);
                var blazingField = instance.GetComponent<Units.VisualEffects.BlazingField>();
                if (blazingField != null)
                {
                    // 假设 BlazingField 有一个方法来初始化效果
                    blazingField.Initialize(
                        context.Radius,
                        context.Duration
                    );
                }
                else
                {
                    Debug.LogWarning("BlazingField component not found on prefab.");
                }
            }
            else if (context.Skill is FlameRing)
            {
                prefab = flamingRingPrefab;
                Vector3 spawnPos = context.Position;
                GameObject instance = Instantiate(prefab, spawnPos, Quaternion.identity);
                var flamingRing = instance.GetComponent<Units.VisualEffects.FlamingRing>();
                if (flamingRing != null)
                {
                    flamingRing.Initialize(context.Target.transform, context.Radius);
                }
                else
                {
                    Debug.LogWarning("FlamingRing component not found on prefab.");
                }
            }
            else if (context.Skill is IceShield)
            {
                prefab = iceShieldPrefab;
                GameObject instance = Instantiate(prefab, context.Caster.transform.position, Quaternion.identity);
                var iceShield = instance.GetComponent<Units.VisualEffects.IceShield>();
                if (iceShield != null)
                {
                    iceShield.Initialize(context.Caster.transform);
                }
                else
                {
                    Debug.LogWarning("IceShield component not found on prefab.");
                }
            }
            else if (context.Skill is FrostZone skillFrostZone)
            {
                prefab = frostZonePrefab;
                GameObject instance = Instantiate(prefab, context.Position, Quaternion.identity);
                var frostZone = instance.GetComponent<Units.VisualEffects.FrostZone>();
                if (frostZone != null)
                {
                    frostZone.Initialize(skillFrostZone.GetRadius(), skillFrostZone.GetDuration());
                }
                else
                {
                    Debug.LogWarning("FrostZone component not found on prefab.");
                }
            }
            else if (context.Skill is IcyCage skillIcyCage)
            {
                prefab = icyCagePrefab;
                GameObject instance = Instantiate(prefab, context.Position, Quaternion.identity);
                var icyCage = instance.GetComponent<Units.VisualEffects.IcyCage>();
                if (icyCage != null)
                {
                    icyCage.Initialize(context.Target.transform, skillIcyCage.GetDuration());
                }
                else
                {
                    Debug.LogWarning("IcyCage component not found on prefab.");
                }
            }
            else
            {
                Debug.LogWarning($"No visual prefab found for skill type: {context.Skill?.GetType().Name}");
                return;
            }


        }
    }
}