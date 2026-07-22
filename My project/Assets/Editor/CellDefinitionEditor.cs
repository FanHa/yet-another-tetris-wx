using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Model.Tetri;
using UnityEditor;
using UnityEngine;

namespace Editor.Validation
{
    [CustomEditor(typeof(CellDefinition))]
    public sealed class CellDefinitionEditor : UnityEditor.Editor
    {
        private static readonly GUIContent IdLabel = new("Id");
        private static readonly GUIContent RuntimeTypeLabel = new("Runtime Type");
        private static readonly GUIContent IconLabel = new("Icon");

        private static List<Type> cachedCellTypes;
        private static List<string> cachedDisplayNames;

        private SerializedProperty idProperty;
        private SerializedProperty runtimeTypeNameProperty;
        private SerializedProperty iconProperty;

        private void OnEnable()
        {
            idProperty = serializedObject.FindProperty("id");
            runtimeTypeNameProperty = serializedObject.FindProperty("runtimeTypeName");
            iconProperty = serializedObject.FindProperty("icon");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EnsureTypeCache();

            DrawRuntimeTypePopup();
            DrawReadOnlyId();
            EditorGUILayout.PropertyField(iconProperty, IconLabel);

            serializedObject.ApplyModifiedProperties();

            if (target is CellDefinition definition && definition.TryGetExpectedId(out string expectedId) && idProperty.stringValue != expectedId)
            {
                idProperty.stringValue = expectedId;
                serializedObject.ApplyModifiedPropertiesWithoutUndo();
            }
        }

        private void DrawRuntimeTypePopup()
        {
            if (cachedCellTypes.Count == 0)
            {
                EditorGUILayout.PropertyField(runtimeTypeNameProperty, RuntimeTypeLabel);
                EditorGUILayout.HelpBox("No concrete Cell types were found. RuntimeTypeName remains editable as text.", MessageType.Warning);
                return;
            }

            int selectedIndex = Mathf.Max(0, cachedCellTypes.FindIndex(type =>
                string.Equals(type.AssemblyQualifiedName, runtimeTypeNameProperty.stringValue, StringComparison.Ordinal)));

            int nextIndex = EditorGUILayout.Popup(RuntimeTypeLabel, selectedIndex, cachedDisplayNames.ToArray());
            nextIndex = Mathf.Clamp(nextIndex, 0, cachedCellTypes.Count - 1);

            if (nextIndex != selectedIndex || string.IsNullOrWhiteSpace(runtimeTypeNameProperty.stringValue))
            {
                runtimeTypeNameProperty.stringValue = cachedCellTypes[nextIndex].AssemblyQualifiedName;
            }
        }

        private void DrawReadOnlyId()
        {
            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.PropertyField(idProperty, IdLabel);
            }
        }

        private static void EnsureTypeCache()
        {
            if (cachedCellTypes != null && cachedDisplayNames != null)
            {
                return;
            }

            cachedCellTypes = AppDomain.CurrentDomain.GetAssemblies()
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
                    typeof(Cell).IsAssignableFrom(type))
                .OrderBy(type => type.FullName)
                .ToList();

            cachedDisplayNames = cachedCellTypes
                .Select(type => type.FullName)
                .ToList();
        }
    }
}