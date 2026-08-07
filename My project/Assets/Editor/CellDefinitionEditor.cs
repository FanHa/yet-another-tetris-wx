using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Model.Tetri;
using UnityEditor;
using UnityEngine;

namespace Editor.Validation
{
    public abstract class CellDefinitionEditorBase : UnityEditor.Editor
    {
        protected static readonly GUIContent IdLabel = new("Id");
        protected static readonly GUIContent RuntimeTypeLabel = new("Runtime Type");
        protected static readonly GUIContent AffinityLabel = new("Affinity");

        private static List<Type> cachedCellTypes;
        private static List<string> cachedDisplayNames;

        protected SerializedProperty idProperty;
        protected SerializedProperty runtimeTypeNameProperty;
        protected SerializedProperty affinityProperty;
        protected SerializedProperty scriptProperty;

        protected virtual void OnEnable()
        {
            scriptProperty = serializedObject.FindProperty("m_Script");
            idProperty = serializedObject.FindProperty("id");
            runtimeTypeNameProperty = serializedObject.FindProperty("runtimeTypeName");
            affinityProperty = serializedObject.FindProperty("affinity");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawScriptField();
            EnsureTypeCache();

            DrawRuntimeTypePopup();
            DrawAffinityField();
            DrawEditableId();
            DrawDerivedFields();

            serializedObject.ApplyModifiedProperties();
        }

        protected abstract void DrawDerivedFields();

        protected void DrawScriptField()
        {
            if (scriptProperty == null)
            {
                return;
            }

            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.PropertyField(scriptProperty);
            }

            EditorGUILayout.Space();
        }

        protected void DrawRuntimeTypePopup()
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

        protected void DrawEditableId()
        {
            EditorGUILayout.PropertyField(idProperty, IdLabel);
        }

        protected void DrawAffinityField()
        {
            if (affinityProperty == null)
            {
                return;
            }

            EditorGUILayout.PropertyField(affinityProperty, AffinityLabel);
        }

        protected static void EnsureTypeCache()
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