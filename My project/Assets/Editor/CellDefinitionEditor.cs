using UnityEditor;
using UnityEngine;

namespace Editor.Validation
{
    public abstract class CellDefinitionEditorBase : UnityEditor.Editor
    {
        protected static readonly GUIContent IdLabel = new("Id");
        protected static readonly GUIContent ResonanceLabel = new("Resonance");

        protected SerializedProperty idProperty;
        protected SerializedProperty resonanceProperty;
        protected SerializedProperty scriptProperty;

        protected virtual void OnEnable()
        {
            scriptProperty = serializedObject.FindProperty("m_Script");
            idProperty = serializedObject.FindProperty("id");
            resonanceProperty = serializedObject.FindProperty("resonance");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawScriptField();
            DrawResonanceField();
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

        protected void DrawResonanceField()
        {
            if (resonanceProperty == null)
            {
                return;
            }

            EditorGUILayout.PropertyField(resonanceProperty, ResonanceLabel);
        }
    }
}