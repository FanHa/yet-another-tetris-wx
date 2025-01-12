using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Model
{
    public abstract class BaseListEditor : Editor
    {

        private Dictionary<string, bool> showLists = new Dictionary<string, bool>();

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
            if (!showLists.ContainsKey(listName))
            {
                showLists[listName] = false;
            }

            showLists[listName] = EditorGUILayout.Foldout(showLists[listName], listName);
       
            if (showLists[listName])
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

        protected void InitializeElement(SerializedProperty elementProperty)
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