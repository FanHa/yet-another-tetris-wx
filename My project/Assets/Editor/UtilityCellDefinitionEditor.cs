using Model.Tetri;
using UnityEditor;
using UnityEngine;

namespace Editor.Validation
{
    [CustomEditor(typeof(UtilityCellDefinition))]
    public sealed class UtilityCellDefinitionEditor : CellDefinitionEditorBase
    {
        private static readonly GUIContent IconLabel = new("Icon");
        private SerializedProperty iconProperty;

        protected override void OnEnable()
        {
            base.OnEnable();
            iconProperty = serializedObject.FindProperty("icon");
        }

        protected override void DrawDerivedFields()
        {
            if (iconProperty != null)
            {
                EditorGUILayout.PropertyField(iconProperty, IconLabel);
            }
        }
    }
}
