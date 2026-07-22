using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Model.Tetri;
using UnityEditor;
using UnityEngine;

namespace Editor.Validation
{
    [InitializeOnLoad]
    public static class CellTypeRegistryStartupValidator
    {
        private const string LogPrefix = "[CellTypeValidator]";
        private static bool hasValidatedThisSession;

        static CellTypeRegistryStartupValidator()
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

            return !hasErrors;
        }

        private static bool ValidateDatabase(CellDatabase database)
        {
            bool hasErrors = false;
            string databasePath = AssetDatabase.GetAssetPath(database);
            List<CellDefinition> definitions = database.GetDefinitions();

            if (definitions.Count == 0)
            {
                Debug.LogWarning($"{LogPrefix} CellDatabase has no definitions: {databasePath}");
                return hasErrors;
            }

            var seenIds = new HashSet<string>(StringComparer.Ordinal);
            var seenTypes = new HashSet<Type>();

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

                if (!seenIds.Add(definition.Id))
                {
                    Debug.LogError($"{LogPrefix} Duplicate CellDefinition id '{definition.Id}' in {databasePath}");
                    hasErrors = true;
                }

                if (!definition.TryResolveCellType(out Type cellType, out string error))
                {
                    Debug.LogError($"{LogPrefix} Invalid CellDefinition type: {definitionPath}. {error}");
                    hasErrors = true;
                    continue;
                }

                if (!seenTypes.Add(cellType))
                {
                    Debug.LogError($"{LogPrefix} Duplicate CellDefinition runtime type '{cellType.FullName}' in {databasePath}");
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

                    if (IsDeprecated(cell.CellTypeId))
                    {
                        Debug.LogError($"{LogPrefix} Deprecated CellTypeId is still registered in factory: {cell.CellTypeId} ({factoryPath})");
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
                if (!database.TryGetSprite(cellId, out Sprite sprite) || sprite == null)
                {
                    Debug.LogWarning($"{LogPrefix} Missing sprite for cell id {cellId} in {AssetDatabase.GetAssetPath(database)}");
                }
            }

            return hasErrors;
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
