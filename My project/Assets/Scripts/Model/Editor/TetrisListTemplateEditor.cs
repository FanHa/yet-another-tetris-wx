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
            unusedTetriListTemplateProperty = serializedObject.FindProperty("template");
        }

        protected override void DrawCustomInspector()
        {
            DrawList(unusedTetriListTemplateProperty, "Unused Tetri List Template");
        }


    }
}