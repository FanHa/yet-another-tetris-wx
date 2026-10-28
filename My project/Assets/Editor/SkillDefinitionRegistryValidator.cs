using System;
using System.Collections.Generic;
using Model.Skills;
using Units.Skills;
using UnityEditor;
using UnityEngine;

namespace Editor.Validation
{
    internal static class SkillDefinitionRegistryValidator
    {
        [MenuItem("Tools/Validation/Validate Skill Definition Registry")]
        private static void ValidateRegistry()
        {
            int errorCount = 0;
            string[] skillGuids = AssetDatabase.FindAssets("t:SkillDefinition");
            HashSet<string> seenSkillIds = new(StringComparer.Ordinal);
            HashSet<string> registeredSkillIds = new(SkillFactory.GetRegisteredSkillIds(), StringComparer.Ordinal);

            foreach (string guid in skillGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                SkillDefinition definition = AssetDatabase.LoadAssetAtPath<SkillDefinition>(path);
                if (definition == null)
                    continue;

                if (string.IsNullOrWhiteSpace(definition.Id))
                {
                    Debug.LogError($"SkillDefinition asset '{path}' has no ID.", definition);
                    errorCount++;
                    continue;
                }

                if (!seenSkillIds.Add(definition.Id))
                {
                    Debug.LogError($"Multiple SkillDefinition assets use ID '{definition.Id}'.", definition);
                    errorCount++;
                }

                if (!registeredSkillIds.Contains(definition.Id))
                {
                    Debug.LogError($"SkillDefinition asset '{path}' has no SkillFactory mapping for ID '{definition.Id}'.", definition);
                    errorCount++;
                    continue;
                }

                if (SkillFactory.TryGetRequiredDefinitionType(definition.Id, out Type requiredDefinitionType) &&
                    !requiredDefinitionType.IsInstanceOfType(definition))
                {
                    Debug.LogError(
                        $"SkillDefinition '{definition.Id}' requires type '{requiredDefinitionType.Name}', " +
                        $"but asset '{path}' has type '{definition.GetType().Name}'.",
                        definition);
                    errorCount++;
                }
            }

            foreach (string skillId in registeredSkillIds)
            {
                if (!seenSkillIds.Contains(skillId))
                {
                    Debug.LogError($"SkillFactory mapping '{skillId}' has no SkillDefinition asset.");
                    errorCount++;
                }
            }

            if (errorCount == 0)
            {
                Debug.Log($"Validated SkillDefinition IDs/types and checked {registeredSkillIds.Count} SkillFactory mappings.");
            }
            else
            {
                Debug.LogError($"Skill definition registry validation found {errorCount} error(s).");
            }
        }
    }
}
