using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Model.Tetri;
using Units.Skills;
using UnityEditor;
using UnityEngine;

namespace Editor.Validation
{
    public static class CellDefinitionMigrationTool
    {
        private const string DefinitionsFolder = "Assets/Data/Cells/Definitions";
        private const string CellDatabasePath = "Assets/Data/Cells/CellDatabase.asset";

        [MenuItem("Tools/Validation/Migrate Cell Definitions From Code")]
        public static void Migrate()
        {
            TetriCellFactory factory = FindFirstAsset<TetriCellFactory>();
            if (factory == null)
            {
                Debug.LogError("[CellDefinitionMigration] TetriCellFactory asset not found.");
                return;
            }

            CellSkillConfigRegistry skillRegistry = FindFirstAsset<CellSkillConfigRegistry>();
            if (skillRegistry == null)
            {
                Debug.LogError("[CellDefinitionMigration] CellSkillConfigRegistry asset not found.");
                return;
            }

            EnsureFolder("Assets/Data");
            EnsureFolder("Assets/Data/Cells");
            EnsureFolder(DefinitionsFolder);

            CellDatabase database = AssetDatabase.LoadAssetAtPath<CellDatabase>(CellDatabasePath);
            if (database == null)
            {
                database = ScriptableObject.CreateInstance<CellDatabase>();
                AssetDatabase.CreateAsset(database, CellDatabasePath);
            }

            AssignDatabaseToFactory(factory, database);

            List<(CellTypeId id, CellDefinition definition)> migratedDefinitions = new();
            foreach (Type cellType in GetCellTypesToMigrate())
            {
                if (!TryCreateSampleCell(cellType, out Cell sampleCell))
                {
                    Debug.LogWarning($"[CellDefinitionMigration] Skip type '{cellType.FullName}' because it cannot be instantiated.");
                    continue;
                }

                if (sampleCell.CellTypeId == CellTypeId.None)
                {
                    continue;
                }

                string definitionAssetPath = $"{DefinitionsFolder}/{sampleCell.CellTypeId}.asset";
                CellDefinition definition = AssetDatabase.LoadAssetAtPath<CellDefinition>(definitionAssetPath);
                if (definition == null)
                {
                    definition = ScriptableObject.CreateInstance<CellDefinition>();
                    AssetDatabase.CreateAsset(definition, definitionAssetPath);
                }

                SkillConfig config = ResolveConfigForCellType(cellType, skillRegistry);
                WriteDefinition(definition, sampleCell, cellType, config);
                migratedDefinitions.Add((sampleCell.CellTypeId, definition));
            }

            migratedDefinitions = migratedDefinitions
                .OrderBy(item => (int)item.id)
                .ToList();

            WriteDatabaseDefinitions(database, migratedDefinitions.Select(item => item.definition).ToList());

            EditorUtility.SetDirty(database);
            EditorUtility.SetDirty(factory);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"[CellDefinitionMigration] Migration completed. Generated/updated {migratedDefinitions.Count} CellDefinition assets.");
        }

        private static void WriteDefinition(CellDefinition definition, Cell sampleCell, Type cellType, SkillConfig config)
        {
            SerializedObject serializedObject = new SerializedObject(definition);
            serializedObject.FindProperty("id").stringValue = sampleCell.CellTypeId.ToString();
            serializedObject.FindProperty("runtimeTypeName").stringValue = cellType.AssemblyQualifiedName;
            serializedObject.FindProperty("config").objectReferenceValue = config;
            serializedObject.FindProperty("affinity").enumValueIndex = (int)sampleCell.Affinity;

            string displayName = SafeGetName(sampleCell);
            string description = SafeGetDescription(sampleCell);
            serializedObject.FindProperty("displayName").stringValue = displayName;
            serializedObject.FindProperty("description").stringValue = description;

            serializedObject.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(definition);
        }

        private static void WriteDatabaseDefinitions(CellDatabase database, List<CellDefinition> definitions)
        {
            SerializedObject serializedObject = new SerializedObject(database);
            SerializedProperty definitionsProperty = serializedObject.FindProperty("definitions");
            definitionsProperty.arraySize = definitions.Count;

            for (int i = 0; i < definitions.Count; i++)
            {
                definitionsProperty.GetArrayElementAtIndex(i).objectReferenceValue = definitions[i];
            }

            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void AssignDatabaseToFactory(TetriCellFactory factory, CellDatabase database)
        {
            SerializedObject serializedObject = new SerializedObject(factory);
            SerializedProperty cellDatabaseProperty = serializedObject.FindProperty("cellDatabase");
            if (cellDatabaseProperty != null)
            {
                cellDatabaseProperty.objectReferenceValue = database;
                serializedObject.ApplyModifiedPropertiesWithoutUndo();
            }
        }

        private static SkillConfig ResolveConfigForCellType(Type cellType, CellSkillConfigRegistry registry)
        {
            if (cellType == typeof(Padding))
            {
                return null;
            }

            string expectedFieldName = $"{cellType.Name}SkillConfig";
            FieldInfo directField = registry.GetType().GetField(expectedFieldName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (directField != null && typeof(SkillConfig).IsAssignableFrom(directField.FieldType))
            {
                return directField.GetValue(registry) as SkillConfig;
            }

            FieldInfo byTypeField = registry.GetType()
                .GetFields(BindingFlags.Public | BindingFlags.Instance)
                .FirstOrDefault(field => field.FieldType.Name.Equals(expectedFieldName, StringComparison.Ordinal));

            return byTypeField?.GetValue(registry) as SkillConfig;
        }

        private static IEnumerable<Type> GetCellTypesToMigrate()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly =>
                {
                    try
                    {
                        return assembly.GetTypes();
                    }
                    catch (ReflectionTypeLoadException ex)
                    {
                        return ex.Types.Where(type => type != null);
                    }
                })
                .Where(type =>
                    type != null &&
                    type.IsClass &&
                    !type.IsAbstract &&
                    typeof(Cell).IsAssignableFrom(type) &&
                    !typeof(Character).IsAssignableFrom(type))
                .OrderBy(type => type.FullName)
                .ToList();
        }

        private static bool TryCreateSampleCell(Type cellType, out Cell cell)
        {
            cell = null;
            try
            {
                cell = (Cell)Activator.CreateInstance(cellType);
                return cell != null;
            }
            catch
            {
                return false;
            }
        }

        private static string SafeGetName(Cell cell)
        {
            try
            {
                return cell.Name() ?? cell.GetType().Name;
            }
            catch
            {
                return cell.GetType().Name;
            }
        }

        private static string SafeGetDescription(Cell cell)
        {
            try
            {
                return cell.Description() ?? string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        private static T FindFirstAsset<T>() where T : UnityEngine.Object
        {
            string[] guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}");
            if (guids.Length == 0)
            {
                return null;
            }

            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            return AssetDatabase.LoadAssetAtPath<T>(path);
        }

        private static void EnsureFolder(string folder)
        {
            if (AssetDatabase.IsValidFolder(folder))
            {
                return;
            }

            string parent = Path.GetDirectoryName(folder)?.Replace('\\', '/');
            string name = Path.GetFileName(folder);
            if (string.IsNullOrEmpty(parent) || string.IsNullOrEmpty(name))
            {
                return;
            }

            if (!AssetDatabase.IsValidFolder(parent))
            {
                EnsureFolder(parent);
            }

            AssetDatabase.CreateFolder(parent, name);
        }
    }
}
