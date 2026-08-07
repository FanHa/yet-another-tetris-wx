using Model.Tetri;
using UnityEditor;
using UnityEngine;

namespace Editor.Validation
{
    [CustomEditor(typeof(SkillBackedCellDefinition))]
    public sealed class SkillBackedCellDefinitionEditor : CellDefinitionEditorBase
    {
        private static readonly GUIContent SkillDefinitionLabel = new("Skill Definition");
        private SerializedProperty skillDefinitionProperty;

        protected override void OnEnable()
        {
            base.OnEnable();
            skillDefinitionProperty = serializedObject.FindProperty("skillDefinition");
        }

        protected override bool ShouldDrawRuntimeTypeField()
        {
            return false;
        }

        protected override void DrawDerivedFields()
        {
            if (skillDefinitionProperty != null)
            {
                EditorGUILayout.PropertyField(skillDefinitionProperty, SkillDefinitionLabel);
            }
        }
    }
}
