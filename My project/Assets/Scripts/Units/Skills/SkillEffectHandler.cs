using System;
using System.Collections.Generic;
using UnityEngine;

namespace Units.Skills
{
    public class SkillEffectHandler : MonoBehaviour
    {
        [SerializeField] private GameObject blazingFieldPrefab;


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
            else
            {
                Debug.LogWarning($"No visual prefab found for skill type: {context.Skill?.GetType().Name}");
                return;
            }


        }
    }
}