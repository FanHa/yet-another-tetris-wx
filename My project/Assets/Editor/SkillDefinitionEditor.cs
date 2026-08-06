using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Model.Tetri;
using Units.Skills;
using UnityEditor;
using UnityEngine;

namespace Editor.Validation
{
    [CustomEditor(typeof(SkillDefinition))]
    public sealed class SkillDefinitionEditor : UnityEditor.Editor
    {
        private static readonly GUIContent IdLabel = new("Id");
        private static readonly GUIContent DisplayNameLabel = new("Display Name");
        private static readonly GUIContent RuntimeTypeLabel = new("Runtime Type");
        private static readonly GUIContent ConfigLabel = new("Config");
        private static readonly GUIContent IconLabel = new("Icon");

        private static List<Type> cachedSkillTypes;
        private static List<string> cachedDisplayNames;

        private SerializedProperty idProperty;
        private SerializedProperty displayNameProperty;
        private SerializedProperty runtimeTypeNameProperty;
        private SerializedProperty configProperty;
        private SerializedProperty iconProperty;

        private void OnEnable()
        {
            idProperty = serializedObject.FindProperty("id");
            displayNameProperty = serializedObject.FindProperty("displayName");
            runtimeTypeNameProperty = serializedObject.FindProperty("runtimeTypeName");
            configProperty = serializedObject.FindProperty("config");
            iconProperty = serializedObject.FindProperty("icon");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EnsureTypeCache();

            DrawRuntimeTypePopup();
            DrawReadOnlyId();
            EditorGUILayout.PropertyField(displayNameProperty, DisplayNameLabel);
            EditorGUILayout.PropertyField(configProperty, ConfigLabel);
            EditorGUILayout.PropertyField(iconProperty, IconLabel);

            serializedObject.ApplyModifiedProperties();

            SyncIdPropertyFromRuntimeType();
        }

        private void DrawRuntimeTypePopup()
        {
            if (cachedSkillTypes.Count == 0)
            {
                EditorGUILayout.PropertyField(runtimeTypeNameProperty, RuntimeTypeLabel);
                EditorGUILayout.HelpBox("No concrete Skill types were found. RuntimeTypeName remains editable as text.", MessageType.Warning);
                return;
            }

            int selectedIndex = cachedSkillTypes.FindIndex(type =>
                string.Equals(type.AssemblyQualifiedName, runtimeTypeNameProperty.stringValue, StringComparison.Ordinal));

            if (selectedIndex < 0)
            {
                selectedIndex = 0;
            }

            int nextIndex = EditorGUILayout.Popup(RuntimeTypeLabel, selectedIndex, cachedDisplayNames.ToArray());
            nextIndex = Mathf.Clamp(nextIndex, 0, cachedSkillTypes.Count - 1);

            if (nextIndex != selectedIndex || string.IsNullOrWhiteSpace(runtimeTypeNameProperty.stringValue))
            {
                runtimeTypeNameProperty.stringValue = cachedSkillTypes[nextIndex].AssemblyQualifiedName;
            }
        }

        private void DrawReadOnlyId()
        {
            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.PropertyField(idProperty, IdLabel);
            }
        }

        private void SyncIdPropertyFromRuntimeType()
        {
            if (idProperty == null || runtimeTypeNameProperty == null)
            {
                return;
            }

            string runtimeTypeName = runtimeTypeNameProperty.stringValue;
            if (string.IsNullOrWhiteSpace(runtimeTypeName))
            {
                return;
            }

            Type runtimeType = Type.GetType(runtimeTypeName, false);
            if (runtimeType == null)
            {
                return;
            }

            string expectedId = runtimeType.Name;
            if (idProperty.stringValue == expectedId)
            {
                return;
            }

            serializedObject.Update();
            idProperty.stringValue = expectedId;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void EnsureTypeCache()
        {
            if (cachedSkillTypes != null && cachedDisplayNames != null)
            {
                return;
            }

            cachedSkillTypes = AppDomain.CurrentDomain.GetAssemblies()
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
                    typeof(Skill).IsAssignableFrom(type))
                .OrderBy(type => type.FullName)
                .ToList();

            cachedDisplayNames = cachedSkillTypes
                .Select(type => type.FullName)
                .ToList();
        }
    }
}
