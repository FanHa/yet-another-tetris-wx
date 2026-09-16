using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Model;
using Model.Tetri;
using Units.Skills;
using UnityEditor;
using UnityEngine;

namespace Editor.Validation
{
    [InitializeOnLoad]
    public static class CellTypeRegistryValidator
    {
        private const string LogPrefix = "[CellTypeValidator]";
        private static readonly Regex CellIdPattern = new("^[A-Za-z][A-Za-z0-9_]*$", RegexOptions.Compiled);
        private static bool hasValidatedThisSession;

        static CellTypeRegistryValidator()
        {
            EditorApplication.delayCall += ValidateOnEditorLoad;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private static void ValidateOnEditorLoad()
        {
            if (hasValidatedThisSession)
            {
                return;
            }

            ValidateAll();
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingEditMode)
            {
                ValidateAll();
            }
        }

        [MenuItem("Tools/Validation/Validate CellType Registry")]
        public static void ValidateAll()
        {
            bool isValid = ValidateAllAndGetResult();
            if (isValid)
            {
                Debug.Log($"{LogPrefix} Validation passed.");
            }
            else
            {
                Debug.LogError($"{LogPrefix} Validation failed.");
            }
        }

        public static bool ValidateAllAndGetResult()
        {
            hasValidatedThisSession = true;
            bool hasErrors = false;

            CellDatabase[] databases = LoadAssets<CellDatabase>();
            TetriCellFactory[] factories = LoadAssets<TetriCellFactory>();
            TetriInventoryInitConfig[] inventoryInitConfigs = LoadAssets<TetriInventoryInitConfig>();

            if (databases.Length == 0)
            {
                Debug.LogWarning($"{LogPrefix} No CellDatabase asset found.");
            }
            else
            {
                foreach (CellDatabase database in databases)
                {
                    hasErrors |= ValidateDatabase(database);
                }
            }

            if (factories.Length == 0)
            {
                Debug.LogWarning($"{LogPrefix} No TetriCellFactory asset found.");
                return !hasErrors;
            }

            foreach (TetriCellFactory factory in factories)
            {
                hasErrors |= ValidateFactory(factory, databases);
            }

            foreach (TetriInventoryInitConfig initConfig in inventoryInitConfigs)
            {
                hasErrors |= ValidateInventoryInitConfig(initConfig, databases);
            }

            return !hasErrors;
        }

        private static bool ValidateInventoryInitConfig(TetriInventoryInitConfig initConfig, CellDatabase[] databases)
        {
            bool hasErrors = false;
            string initConfigPath = AssetDatabase.GetAssetPath(initConfig);
            HashSet<string> registeredIds = new(databases.SelectMany(database => database.GetRegisteredCellIds()), StringComparer.Ordinal);

            if (initConfig.CellDefinitions == null)
            {
                Debug.LogWarning($"{LogPrefix} TetriInventoryInitConfig CellDefinitions list is null: {initConfigPath}");
                return false;
            }

            foreach (CellDefinition definition in initConfig.CellDefinitions)
            {
                if (definition == null)
                {
                    Debug.LogError($"{LogPrefix} TetriInventoryInitConfig contains a null CellDefinition: {initConfigPath}");
                    hasErrors = true;
                    continue;
                }

                if (!registeredIds.Contains(definition.Id))
                {
                    Debug.LogError($"{LogPrefix} TetriInventoryInitConfig references CellDefinition '{definition.Id}' that is not registered in any CellDatabase: {initConfigPath}");
                    hasErrors = true;
                }
            }

            return hasErrors;
        }

        private static bool ValidateDatabase(CellDatabase database)
        {
            bool hasErrors = false;
            string databasePath = AssetDatabase.GetAssetPath(database);
            List<CellDefinition> definitions = database.GetRegisteredDefinitions();

            if (definitions.Count == 0)
            {
                Debug.LogWarning($"{LogPrefix} CellDatabase has no definitions: {databasePath}");
                return hasErrors;
            }

            var seenIds = new HashSet<string>(StringComparer.Ordinal);

            foreach (CellDefinition definition in definitions)
            {
                if (definition == null)
                {
                    Debug.LogError($"{LogPrefix} CellDatabase contains a null definition: {databasePath}");
                    hasErrors = true;
                    continue;
                }

                string definitionPath = AssetDatabase.GetAssetPath(definition);
                if (string.IsNullOrWhiteSpace(definition.Id))
                {
                    Debug.LogError($"{LogPrefix} CellDefinition has empty id: {definitionPath}");
                    hasErrors = true;
                    continue;
                }

                if (!string.Equals(definition.Id.Trim(), definition.Id, StringComparison.Ordinal))
                {
                    Debug.LogError($"{LogPrefix} CellDefinition id has leading/trailing spaces '{definition.Id}': {definitionPath}");
                    hasErrors = true;
                }

                if (!CellIdPattern.IsMatch(definition.Id))
                {
                    Debug.LogError($"{LogPrefix} CellDefinition id '{definition.Id}' is invalid. Expected pattern: {CellIdPattern}: {definitionPath}");
                    hasErrors = true;
                }

                if (!seenIds.Add(definition.Id))
                {
                    Debug.LogError($"{LogPrefix} Duplicate CellDefinition id '{definition.Id}' in {databasePath}");
                    hasErrors = true;
                }

                if (definition is not CharacterDefinition && definition.Affinity == AffinityType.None)
                {
                    Debug.LogError($"{LogPrefix} CellDefinition '{definition.name}' has invalid Affinity None: {definitionPath}");
                    hasErrors = true;
                }

                if (definition is SkillBackedCellDefinition skillCellDefinition)
                {

                    if (skillCellDefinition.Affinity == AffinityType.None)
                    {
                        Debug.LogError($"{LogPrefix} SkillBackedCellDefinition has invalid Affinity None: {definitionPath}");
                        hasErrors = true;
                    }

                    if (skillCellDefinition.SkillDefinition == null)
                    {
                        Debug.LogError($"{LogPrefix} SkillBackedCellDefinition is missing SkillDefinition: {definitionPath}");
                        hasErrors = true;
                        continue;
                    }

                    if (skillCellDefinition.SkillDefinition.Config == null)
                    {
                        Debug.LogError($"{LogPrefix} SkillDefinition '{skillCellDefinition.SkillDefinition.name}' has null Config: {definitionPath}");
                        hasErrors = true;
                    }
                    else
                    {
                        hasErrors |= ValidateSkillDefinition(skillCellDefinition.SkillDefinition, definitionPath);
                    }

                    if (string.IsNullOrWhiteSpace(skillCellDefinition.SkillDefinition.DisplayName))
                    {
                        Debug.LogError($"{LogPrefix} SkillDefinition '{skillCellDefinition.SkillDefinition.name}' has empty DisplayName: {definitionPath}");
                        hasErrors = true;
                    }

                    if (string.IsNullOrWhiteSpace(skillCellDefinition.SkillDefinition.Description))
                    {
                        Debug.LogError($"{LogPrefix} SkillDefinition '{skillCellDefinition.SkillDefinition.name}' has empty Description: {definitionPath}");
                        hasErrors = true;
                    }

                    if (skillCellDefinition.SkillDefinition.Icon == null)
                    {
                        Debug.LogError($"{LogPrefix} SkillDefinition '{skillCellDefinition.SkillDefinition.name}' has null Icon: {definitionPath}");
                        hasErrors = true;
                    }

                    continue;
                }

                if (definition is UtilityCellDefinition)
                {
                    if (definition.Icon == null)
                    {
                        Debug.LogError($"{LogPrefix} UtilityCellDefinition has null Icon: {definitionPath}");
                        hasErrors = true;
                    }

                    continue;
                }

                if (definition.Icon == null)
                {
                    Debug.LogError($"{LogPrefix} CellDefinition has null Icon: {definitionPath}");
                    hasErrors = true;
                }
            }

            return hasErrors;
        }

        private static bool ValidateSkillDefinition(SkillDefinition skillDefinition, string definitionPath)
        {
            bool hasErrors = false;
            Type skillType = typeof(Units.Skills.Skill).Assembly.GetType($"Units.Skills.{skillDefinition.Id}");
            if (skillType == null || !typeof(Units.Skills.Skill).IsAssignableFrom(skillType))
            {
                Debug.LogError($"{LogPrefix} SkillDefinition '{skillDefinition.name}' has no matching Skill type for id '{skillDefinition.Id}': {definitionPath}");
                hasErrors = true;
            }

            if (!Enum.TryParse(skillDefinition.Id, out CellTypeId cellTypeId) || cellTypeId == CellTypeId.None)
            {
                Debug.LogError($"{LogPrefix} SkillDefinition '{skillDefinition.name}' has invalid skill id '{skillDefinition.Id}': {definitionPath}");
                hasErrors = true;
            }

            if (skillDefinition.Config is SkillConfig<SkillLevelConfig> config && (config.LevelConfigs == null || config.LevelConfigs.Count == 0))
            {
                Debug.LogError($"{LogPrefix} SkillDefinition '{skillDefinition.name}' has no level configs: {definitionPath}");
                hasErrors = true;
            }

            if (skillType != null && skillDefinition.Config != null && skillType.GetConstructor(Type.EmptyTypes) == null)
            {
                Type levelConfigType = skillDefinition.Config.GetType().BaseType?.GetGenericArguments()[0];
                if (levelConfigType == null || skillType.GetConstructor(new[] { levelConfigType }) == null)
                {
                    Debug.LogError($"{LogPrefix} SkillDefinition '{skillDefinition.name}' has no matching Skill constructor: {definitionPath}");
                    hasErrors = true;
                }
            }

            return hasErrors;
        }

        private static bool ValidateFactory(TetriCellFactory factory, CellDatabase[] databases)
        {
            bool hasErrors = false;
            string factoryPath = AssetDatabase.GetAssetPath(factory);
            CellDatabase database = databases.FirstOrDefault();
            List<string> registeredIds = database != null ? database.GetRegisteredCellIds() : new List<string>();

            if (registeredIds.Count == 0)
            {
                Debug.LogError($"{LogPrefix} Factory has no registered Cell ids: {factoryPath}");
                return true;
            }

            foreach (string cellId in registeredIds)
            {
                try
                {
                    Cell cell = factory.CreateCell(cellId);
                    if (cell == null)
                    {
                        Debug.LogError($"{LogPrefix} Factory returned null for cell id {cellId} ({factoryPath})");
                        hasErrors = true;
                        continue;
                    }

                    if (TryParseLegacyCellTypeId(cellId, out CellTypeId legacyCellTypeId) && IsDeprecated(legacyCellTypeId))
                    {
                        Debug.LogError($"{LogPrefix} Deprecated legacy CellTypeId is still registered in factory: {legacyCellTypeId} ({factoryPath})");
                        hasErrors = true;
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"{LogPrefix} Factory failed to create cell id {cellId} ({factoryPath}): {ex.Message}");
                    hasErrors = true;
                }
            }

            if (database == null)
            {
                Debug.LogWarning($"{LogPrefix} No CellDatabase asset found for sprite validation.");
                return hasErrors;
            }

            foreach (string cellId in registeredIds)
            {
                Sprite sprite = database.GetSprite(cellId);
                if (sprite == null)
                {
                    Debug.LogError($"{LogPrefix} Missing resolved sprite for cell id {cellId} in {AssetDatabase.GetAssetPath(database)}. If this cell is skill-based, ensure bound SkillDefinition has Icon; otherwise set CellDefinition.Icon.");
                    hasErrors = true;
                }
            }

            return hasErrors;
        }

        private static bool TryParseLegacyCellTypeId(string cellId, out CellTypeId cellTypeId)
        {
            if (string.IsNullOrWhiteSpace(cellId))
            {
                cellTypeId = default;
                return false;
            }

            return Enum.TryParse(cellId, out cellTypeId) && Enum.IsDefined(typeof(CellTypeId), cellTypeId);
        }

        private static bool IsDeprecated(CellTypeId cellTypeId)
        {
            string name = Enum.GetName(typeof(CellTypeId), cellTypeId);
            if (string.IsNullOrEmpty(name))
            {
                return false;
            }

            MemberInfo member = typeof(CellTypeId).GetMember(name).FirstOrDefault();
            if (member == null)
            {
                return false;
            }

            return member.GetCustomAttribute<ObsoleteAttribute>() != null;
        }

        private static T[] LoadAssets<T>() where T : UnityEngine.Object
        {
            string filter = $"t:{typeof(T).Name}";
            string[] guids = AssetDatabase.FindAssets(filter);
            return guids
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(path => AssetDatabase.LoadAssetAtPath<T>(path))
                .Where(asset => asset != null)
                .ToArray();
        }
    }
}