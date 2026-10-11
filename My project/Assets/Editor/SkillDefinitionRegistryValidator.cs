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

            foreach (string guid in skillGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                SkillDefinition definition = AssetDatabase.LoadAssetAtPath<SkillDefinition>(path);
                if (definition == null)
                    continue;

                if (!string.IsNullOrWhiteSpace(definition.Id))
                    seenSkillIds.Add(definition.Id);

                if (SkillFactory.TryGetRequiredConfigType(definition.Id, out Type requiredConfigType) &&
                    (definition.Config == null || !requiredConfigType.IsInstanceOfType(definition.Config)))
                {
                    Debug.LogError(
                        $"SkillDefinition '{definition.Id}' requires config type '{requiredConfigType.Name}', " +
                        $"but references '{definition.Config?.GetType().Name ?? "<null>"}'.",
                        definition);
                    errorCount++;
                }
            }

            int registeredSkillCount = 0;
            foreach (string skillId in SkillFactory.GetRegisteredSkillIds())
            {
                registeredSkillCount++;
                if (!seenSkillIds.Contains(skillId))
                {
                    Debug.LogError($"SkillFactory mapping '{skillId}' has no SkillDefinition asset.");
                    errorCount++;
                }
            }

            if (errorCount == 0)
            {
                Debug.Log($"Validated SkillConfig types for SkillDefinition assets and checked {registeredSkillCount} SkillFactory mappings.");
            }
            else
            {
                Debug.LogError($"Skill definition registry validation found {errorCount} error(s).");
            }
        }
    }
}
