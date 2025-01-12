using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace Model
{
    [CustomEditor(typeof(TetrisResources))]
    public class TetrisResourcesEditor : BaseListEditor
    {
        private SerializedProperty tetriListProperty;
        private SerializedProperty usedTetriListProperty;
        private SerializedProperty unusedTetriListProperty;

        protected override void InitializeProperties()
        {
            tetriListProperty = serializedObject.FindProperty("tetriList");
            usedTetriListProperty = serializedObject.FindProperty("usedTetriList");
            unusedTetriListProperty = serializedObject.FindProperty("unusedTetriList");
        }

        protected override void DrawCustomInspector()
        {
            DrawList(tetriListProperty, "Tetri List");
            EditorGUILayout.Space();

            DrawList(usedTetriListProperty, "Used Tetri List");
            EditorGUILayout.Space();

            DrawList(unusedTetriListProperty, "Unused Tetri List");
            EditorGUILayout.Space();
        }

    }
}