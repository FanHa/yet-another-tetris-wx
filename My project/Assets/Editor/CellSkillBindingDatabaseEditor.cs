using System;
using System.Collections.Generic;
using System.Linq;
using Model.Tetri;
using UnityEditor;
using UnityEngine;

namespace Editor.Validation
{
    [CustomEditor(typeof(CellSkillBindingDatabase))]
    public sealed class CellSkillBindingDatabaseEditor : UnityEditor.Editor
    {
        private static readonly GUIContent CellIdLabel = new("Cell Id");

        private SerializedProperty bindingsProperty;

        private void OnEnable()
        {
            bindingsProperty = serializedObject.FindProperty("bindings");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            List<string> availableCellIds = LoadRegisteredCellIds();
            DrawBindings(availableCellIds);
            DrawToolbar();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawBindings(List<string> availableCellIds)
        {
            if (bindingsProperty == null)
            {
                EditorGUILayout.HelpBox("Cannot find serialized property: bindings", MessageType.Error);
                return;
            }

            if (bindingsProperty.arraySize == 0)
            {
                EditorGUILayout.HelpBox("No bindings configured.", MessageType.Info);
                return;
            }

            for (int i = 0; i < bindingsProperty.arraySize; i++)
            {
                SerializedProperty itemProperty = bindingsProperty.GetArrayElementAtIndex(i);
                SerializedProperty cellIdProperty = itemProperty.FindPropertyRelative("cellId");
                SerializedProperty skillConfigProperty = itemProperty.FindPropertyRelative("skillConfig");

                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.LabelField($"Binding {i}", EditorStyles.boldLabel);

                DrawCellIdSelector(cellIdProperty, availableCellIds);
                EditorGUILayout.PropertyField(skillConfigProperty);

                if (GUILayout.Button("Remove"))
                {
                    bindingsProperty.DeleteArrayElementAtIndex(i);
                    EditorGUILayout.EndVertical();
                    break;
                }

                EditorGUILayout.EndVertical();
            }
        }

        private void DrawCellIdSelector(SerializedProperty cellIdProperty, List<string> availableCellIds)
        {
            string currentValue = cellIdProperty.stringValue;

            if (availableCellIds.Count == 0)
            {
                EditorGUILayout.PropertyField(cellIdProperty, CellIdLabel);
                EditorGUILayout.HelpBox("No CellDatabase entries found. Configure CellDatabase first.", MessageType.Warning);
                return;
            }

            List<string> options = new List<string>(availableCellIds);
            bool hasCurrent = !string.IsNullOrWhiteSpace(currentValue) && options.Contains(currentValue, StringComparer.Ordinal);

            if (!hasCurrent && !string.IsNullOrWhiteSpace(currentValue))
            {
                options.Add(currentValue);
            }

            int currentIndex = Mathf.Max(0, options.FindIndex(id => string.Equals(id, currentValue, StringComparison.Ordinal)));
            int nextIndex = EditorGUILayout.Popup(CellIdLabel, currentIndex, options.ToArray());
            nextIndex = Mathf.Clamp(nextIndex, 0, options.Count - 1);

            string selected = options[nextIndex];
            if (!string.Equals(selected, currentValue, StringComparison.Ordinal))
            {
                cellIdProperty.stringValue = selected;
            }

            if (!hasCurrent && !string.IsNullOrWhiteSpace(currentValue))
            {
                EditorGUILayout.HelpBox($"Current CellId '{currentValue}' is not registered in any CellDatabase.", MessageType.Warning);
            }
        }

        private void DrawToolbar()
        {
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Add Binding"))
            {
                bindingsProperty.InsertArrayElementAtIndex(bindingsProperty.arraySize);
                SerializedProperty item = bindingsProperty.GetArrayElementAtIndex(bindingsProperty.arraySize - 1);
                item.FindPropertyRelative("cellId").stringValue = string.Empty;
                item.FindPropertyRelative("skillConfig").objectReferenceValue = null;
            }

            EditorGUILayout.EndHorizontal();
        }

        private static List<string> LoadRegisteredCellIds()
        {
            string[] databaseGuids = AssetDatabase.FindAssets("t:CellDatabase");
            if (databaseGuids == null || databaseGuids.Length == 0)
            {
                return new List<string>();
            }

            HashSet<string> idSet = new(StringComparer.Ordinal);

            foreach (string guid in databaseGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                CellDatabase database = AssetDatabase.LoadAssetAtPath<CellDatabase>(path);
                if (database == null)
                {
                    continue;
                }

                foreach (string id in database.GetRegisteredCellIds())
                {
                    if (!string.IsNullOrWhiteSpace(id))
                    {
                        idSet.Add(id);
                    }
                }
            }

            return idSet
                .OrderBy(id => id, StringComparer.Ordinal)
                .ToList();
        }
    }
}
