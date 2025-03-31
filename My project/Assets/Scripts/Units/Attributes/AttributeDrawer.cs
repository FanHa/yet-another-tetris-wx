using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Units.Editor
{
    [CustomPropertyDrawer(typeof(Attribute))]
    public class AttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            // 获取字段
            SerializedProperty baseValueProp = property.FindPropertyRelative("baseValue");
            SerializedProperty flatModifiersProp = property.FindPropertyRelative("flatModifiers");
            SerializedProperty percentageModifiersProp = property.FindPropertyRelative("percentageModifiers");
            SerializedProperty valueProp = property.FindPropertyRelative("finalValue");
            // 绘制属性名称
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            // 绘制基础值
            Rect baseValueRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(baseValueRect, baseValueProp, new GUIContent("Base Value"));

            // 绘制最终值（只读）
            Rect valueRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + 2, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(valueRect, "Final Value", valueProp.floatValue.ToString());

            // 绘制 Flat Modifiers
            Rect flatModifiersRect = new Rect(position.x, position.y + (EditorGUIUtility.singleLineHeight + 2) * 2, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(flatModifiersRect, "Flat Modifiers");

            // 展开 Flat Modifiers
            if (flatModifiersProp != null)
            {
                EditorGUI.indentLevel++;
                foreach (KeyValuePair<object, float> kvp in flatModifiersProp)
                {
                    EditorGUILayout.LabelField(kvp.Key.ToString(), kvp.Value.ToString());
                }
                EditorGUI.indentLevel--;
            }

            // 绘制 Percentage Modifiers
            Rect percentageModifiersRect = new Rect(position.x, position.y + (EditorGUIUtility.singleLineHeight + 2) * 3, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(percentageModifiersRect, "Percentage Modifiers");

            // 展开 Percentage Modifiers
            if (percentageModifiersProp != null)
            {
                EditorGUI.indentLevel++;
                foreach (KeyValuePair<object, float> kvp in percentageModifiersProp)
                {
                    EditorGUILayout.LabelField(kvp.Key.ToString(), kvp.Value.ToString());
                }
                EditorGUI.indentLevel--;
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            // 计算属性高度
            return EditorGUIUtility.singleLineHeight * 6 + 10;
        }
    }
}