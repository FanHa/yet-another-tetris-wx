using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace Model
{
    [CustomEditor(typeof(TetrisResources))]
    public class TetrisResourcesEditor : Editor
    {
        private SerializedProperty tetriListProperty;
        private SerializedProperty usedTetriListProperty;
        private SerializedProperty unusedTetriListProperty;
        private SerializedProperty unusedTetriListTemplateProperty;

        private bool showTetriList = true;
        private bool showUsedTetriList = true;
        private bool showUnusedTetriList = true;
        private bool showUnusedTetriListTemplate = true;

        private void OnEnable()
        {
            tetriListProperty = serializedObject.FindProperty("tetriList");
            usedTetriListProperty = serializedObject.FindProperty("usedTetriList");
            unusedTetriListProperty = serializedObject.FindProperty("unusedTetriList");
            unusedTetriListTemplateProperty = serializedObject.FindProperty("unusedTetriListTemplate");

        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            showTetriList = EditorGUILayout.Foldout(showTetriList, "Tetri List");
            if (showTetriList)
            {
                DrawList(tetriListProperty);
            }

            showUsedTetriList = EditorGUILayout.Foldout(showUsedTetriList, "Used Tetri List");
            if (showUsedTetriList)
            {
                DrawList(usedTetriListProperty);
            }

            showUnusedTetriList = EditorGUILayout.Foldout(showUnusedTetriList, "Unused Tetri List");
            if (showUnusedTetriList)
            {
                DrawList(unusedTetriListProperty);
            }

            showUnusedTetriListTemplate = EditorGUILayout.Foldout(showUnusedTetriListTemplate, "Unused Tetri List Template");
            if (showUnusedTetriListTemplate)
            {
                DrawList(unusedTetriListTemplateProperty);
            }
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawList(SerializedProperty listProperty)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("", GUILayout.Width(15f)); // 缩进
            if (GUILayout.Button("+", GUILayout.Width(20f)))
            {
                listProperty.arraySize++;
                SerializedProperty newElement = listProperty.GetArrayElementAtIndex(listProperty.arraySize - 1);
                InitializeTetri(newElement);
            }
            EditorGUILayout.EndHorizontal();

            for (int i = 0; i < listProperty.arraySize; i++)
            {
                SerializedProperty elementProperty = listProperty.GetArrayElementAtIndex(i);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("", GUILayout.Width(15f)); // 缩进
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(elementProperty, new GUIContent($"Tetri {i + 1}"));
                EditorGUI.indentLevel--;

                if (GUILayout.Button("-", GUILayout.Width(20f)))
                {
                    listProperty.DeleteArrayElementAtIndex(i);
                }
                EditorGUILayout.EndHorizontal();
            }

        }

        private void InitializeTetri(SerializedProperty tetriProperty)
        {
            SerializedProperty shapeProperty = tetriProperty.FindPropertyRelative("shape");
            shapeProperty.FindPropertyRelative("rows").intValue = 4;
            shapeProperty.FindPropertyRelative("cols").intValue = 4;
            shapeProperty.FindPropertyRelative("array").arraySize = 16;
            for (int i = 0; i < 16; i++)
            {
                shapeProperty.FindPropertyRelative("array").GetArrayElementAtIndex(i).intValue = 0;
            }
        }

    }
}