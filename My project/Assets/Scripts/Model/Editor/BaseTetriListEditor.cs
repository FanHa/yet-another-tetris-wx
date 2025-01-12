using UnityEditor;
using UnityEngine;

namespace Model
{
    public abstract class BaseListEditor : Editor
    {

        private bool showList = false;

        protected virtual string ListName => "List";

        private void OnEnable()
        {
            InitializeProperties();
        }

        protected abstract void InitializeProperties();

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

             DrawCustomInspector();

            serializedObject.ApplyModifiedProperties();
        }

        protected virtual void DrawCustomInspector()
        {
            // 默认实现为空，子类可以重写此方法来绘制自定义的Inspector
        }

        protected void DrawList(SerializedProperty listProperty, string listName)
        {
            showList = EditorGUILayout.Foldout(showList, listName);
            if (showList)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("", GUILayout.Width(15f)); // 缩进
                if (GUILayout.Button("+", GUILayout.Width(20f)))
                {
                    listProperty.arraySize++;
                    SerializedProperty newElement = listProperty.GetArrayElementAtIndex(listProperty.arraySize - 1);
                    InitializeElement(newElement);
                }
                EditorGUILayout.EndHorizontal();

                for (int i = 0; i < listProperty.arraySize; i++)
                {
                    SerializedProperty elementProperty = listProperty.GetArrayElementAtIndex(i);

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("", GUILayout.Width(15f)); // 缩进
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(elementProperty, new GUIContent($"Element {i + 1}"));
                    EditorGUI.indentLevel--;

                    if (GUILayout.Button("-", GUILayout.Width(20f)))
                    {
                        listProperty.DeleteArrayElementAtIndex(i);
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
        }

        protected abstract void InitializeElement(SerializedProperty elementProperty);
    }
}