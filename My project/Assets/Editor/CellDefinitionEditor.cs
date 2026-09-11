using Model.Tetri;
using UnityEditor;
using UnityEngine;

namespace Editor.Validation
{
    public abstract class CellDefinitionEditorBase : UnityEditor.Editor
    {
        protected static readonly GUIContent IdLabel = new("Id");
        protected static readonly GUIContent AffinityLabel = new("Affinity");

        protected SerializedProperty idProperty;
        protected SerializedProperty affinityProperty;
        protected SerializedProperty scriptProperty;

        protected virtual void OnEnable()
        {
            scriptProperty = serializedObject.FindProperty("m_Script");
            idProperty = serializedObject.FindProperty("id");
            affinityProperty = serializedObject.FindProperty("affinity");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawScriptField();
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
    }
}