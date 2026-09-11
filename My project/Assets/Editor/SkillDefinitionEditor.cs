using Model.Tetri;
using UnityEditor;
using UnityEngine;

namespace Editor.Validation
{
    [CustomEditor(typeof(SkillDefinition))]
    public sealed class SkillDefinitionEditor : UnityEditor.Editor
    {
        private static readonly GUIContent IdLabel = new("Id");
        private static readonly GUIContent DisplayNameLabel = new("Display Name");
        private static readonly GUIContent DescriptionLabel = new("Description");
        private static readonly GUIContent ConfigLabel = new("Config");
        private static readonly GUIContent IconLabel = new("Icon");

        private SerializedProperty idProperty;
        private SerializedProperty displayNameProperty;
        private SerializedProperty descriptionProperty;
        private SerializedProperty configProperty;
        private SerializedProperty iconProperty;

        private void OnEnable()
        {
            idProperty = serializedObject.FindProperty("id");
            displayNameProperty = serializedObject.FindProperty("displayName");
            descriptionProperty = serializedObject.FindProperty("description");
            configProperty = serializedObject.FindProperty("config");
            iconProperty = serializedObject.FindProperty("icon");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawReadOnlyId();
            EditorGUILayout.PropertyField(displayNameProperty, DisplayNameLabel);
            EditorGUILayout.PropertyField(descriptionProperty, DescriptionLabel);
            EditorGUILayout.PropertyField(configProperty, ConfigLabel);
            EditorGUILayout.PropertyField(iconProperty, IconLabel);

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawReadOnlyId()
        {
            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.PropertyField(idProperty, IdLabel);
            }
        }
    }
}
