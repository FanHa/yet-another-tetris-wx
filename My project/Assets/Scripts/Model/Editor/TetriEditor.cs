using System.Data;
using UnityEditor;
using UnityEngine;
using Model.Tetri;

namespace Model{
    [CustomPropertyDrawer(typeof(Tetri.Tetri))]
    public class TetriPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            
            // 绘制标签
            EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            // 缩进内容
            float indent = 15f;
            position.x += indent;
            position.width -= indent;

            // 获取 shape 属性
            SerializedProperty shapeProperty = property.FindPropertyRelative("shape");
            SerializedProperty rowsProperty = shapeProperty.FindPropertyRelative("rows");
            SerializedProperty colsProperty = shapeProperty.FindPropertyRelative("cols");
            SerializedProperty arrayProperty = shapeProperty.FindPropertyRelative("array");

            int rows = rowsProperty.intValue;
            int cols = colsProperty.intValue;

            float cellWidth = position.width / cols;
            float cellHeight = EditorGUIUtility.singleLineHeight;

            position.y += EditorGUIUtility.singleLineHeight;

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    int index = i * cols + j;
                    SerializedProperty cellProperty = arrayProperty.GetArrayElementAtIndex(index);
                    Rect cellRect = new Rect(position.x + j * cellWidth, position.y + i * cellHeight, cellWidth, cellHeight);
                    cellProperty.intValue = EditorGUI.IntField(cellRect, cellProperty.intValue);
                }
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            // 获取 shape 属性
            SerializedProperty shapeProperty = property.FindPropertyRelative("shape");
            SerializedProperty rowsProperty = shapeProperty.FindPropertyRelative("rows");

            int rows = rowsProperty.intValue;
            return EditorGUIUtility.singleLineHeight * (rows + 2); // 加上标签的高度
        }
    }
}