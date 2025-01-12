using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using Model;

namespace Model
{
    [CustomEditor(typeof(TetrisListTemplate))]
    public class TetrisListTemplateEditor : BaseListEditor
    {
        private SerializedProperty unusedTetriListTemplateProperty;

        protected override void InitializeProperties()
        {
            unusedTetriListTemplateProperty = serializedObject.FindProperty("unusedTetriListTemplate");
        }

        protected override void DrawCustomInspector()
        {
            DrawList(unusedTetriListTemplateProperty, "Unused Tetri List Template");
        }

        protected override void InitializeElement(SerializedProperty elementProperty)
        {
            SerializedProperty shapeProperty = elementProperty.FindPropertyRelative("shape");
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