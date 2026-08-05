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
        private static readonly GUIContent CellDefinitionLabel = new("Cell Definition");

        private SerializedProperty bindingsProperty;

        private void OnEnable()
        {
            bindingsProperty = serializedObject.FindProperty("bindings");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawBindings();
            DrawToolbar();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawBindings()
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
                SerializedProperty cellDefinitionProperty = itemProperty.FindPropertyRelative("cellDefinition");
                SerializedProperty skillDefinitionProperty = itemProperty.FindPropertyRelative("skillDefinition");

                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.LabelField($"Binding {i}", EditorStyles.boldLabel);

                EditorGUILayout.PropertyField(cellDefinitionProperty, CellDefinitionLabel);
                EditorGUILayout.PropertyField(skillDefinitionProperty);

                DrawBindingWarnings(cellDefinitionProperty, skillDefinitionProperty);

                if (GUILayout.Button("Remove"))
                {
                    bindingsProperty.DeleteArrayElementAtIndex(i);
                    EditorGUILayout.EndVertical();
                    break;
                }

                EditorGUILayout.EndVertical();
            }
        }

        private static void DrawBindingWarnings(SerializedProperty cellDefinitionProperty, SerializedProperty skillDefinitionProperty)
        {
            if (cellDefinitionProperty.objectReferenceValue == null)
            {
                EditorGUILayout.HelpBox("CellDefinition is required.", MessageType.Warning);
            }

            if (skillDefinitionProperty.objectReferenceValue == null)
            {
                EditorGUILayout.HelpBox("SkillDefinition is required.", MessageType.Warning);
            }
        }

        private void DrawToolbar()
        {
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Add Binding"))
            {
                bindingsProperty.InsertArrayElementAtIndex(bindingsProperty.arraySize);
                SerializedProperty item = bindingsProperty.GetArrayElementAtIndex(bindingsProperty.arraySize - 1);
                item.FindPropertyRelative("cellDefinition").objectReferenceValue = null;
                item.FindPropertyRelative("skillDefinition").objectReferenceValue = null;
            }

            EditorGUILayout.EndHorizontal();
        }
    }
}
