using System;
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
            hasValidatedThisSession = true;

            TetriCellFactory[] factories = LoadAssets<TetriCellFactory>();
            TetriCellTypeResourceMapping[] mappings = LoadAssets<TetriCellTypeResourceMapping>();

            if (factories.Length == 0)
            {
                Debug.LogWarning($"{LogPrefix} No TetriCellFactory asset found.");
                return;
            }

            foreach (TetriCellFactory factory in factories)
            {
                ValidateFactory(factory, mappings);
            }
        }

        private static void ValidateFactory(TetriCellFactory factory, TetriCellTypeResourceMapping[] mappings)
        {
            string factoryPath = AssetDatabase.GetAssetPath(factory);
            var playableIds = factory.GetRegisteredPlayableCellTypeIds();

            if (playableIds.Count == 0)
            {
                Debug.LogError($"{LogPrefix} Factory has no playable ids: {factoryPath}");
                return;
            }

            foreach (CellTypeId cellTypeId in playableIds)
            {
                if (IsDeprecated(cellTypeId))
                {
                    Debug.LogError($"{LogPrefix} Deprecated CellTypeId is still registered in factory: {cellTypeId} ({factoryPath})");
                }

                try
                {
                    Cell cell = factory.CreateCell(cellTypeId);
                    if (cell == null)
                    {
                        Debug.LogError($"{LogPrefix} Factory returned null for id {cellTypeId} ({factoryPath})");
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"{LogPrefix} Factory failed to create id {cellTypeId} ({factoryPath}): {ex.Message}");
                }
            }

            if (mappings.Length == 0)
            {
                Debug.LogWarning($"{LogPrefix} No TetriCellTypeResourceMapping asset found.");
                return;
            }

            foreach (TetriCellTypeResourceMapping mapping in mappings)
            {
                string mappingPath = AssetDatabase.GetAssetPath(mapping);
                foreach (CellTypeId cellTypeId in playableIds)
                {
                    Sprite sprite = mapping.GetSprite(cellTypeId);
                    if (sprite == null)
                    {
                        Debug.LogWarning($"{LogPrefix} Missing sprite mapping for {cellTypeId} in {mappingPath}");
                    }
                }
            }
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
