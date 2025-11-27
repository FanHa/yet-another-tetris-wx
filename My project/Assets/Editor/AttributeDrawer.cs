using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Editor2
{
    [CustomPropertyDrawer(typeof(Units.Attribute))]
    public class AttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            // 获取字段
            SerializedProperty baseValueProp = property.FindPropertyRelative("baseValue");
            SerializedProperty valueProp = property.FindPropertyRelative("finalValue");

            // 使用反射获取 flatModifiers 和 percentageModifiers 的内容
            object targetObject = GetTargetObjectOfProperty(property);
            FieldInfo flatModifiersField = targetObject.GetType().GetField("flatModifiers", BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo percentageModifiersField = targetObject.GetType().GetField("percentageModifiers", BindingFlags.NonPublic | BindingFlags.Instance);

            var flatModifiers = flatModifiersField?.GetValue(targetObject) as Dictionary<object, float>;
            var percentageModifiers = percentageModifiersField?.GetValue(targetObject) as Dictionary<object, float>;

            // 绘制属性名称
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            // 绘制基础值
            Rect baseValueRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(baseValueRect, baseValueProp, new GUIContent("Base Value"));

            // 绘制最终值（只读）
            Rect valueRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + 2, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(valueRect, "Final Value", valueProp.floatValue.ToString());

            // 更新 position.y，确保 Flat Modifiers 的显示位置正确
            position.y += EditorGUIUtility.singleLineHeight * 2 + 6; // 累加 Final Value 的高度和额外间距

            // 绘制 Flat Modifiers
            Rect flatModifiersLabelRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(flatModifiersLabelRect, "Flat Modifiers");

            if (flatModifiers != null)
            {
                EditorGUI.indentLevel++;
                foreach (var kvp in flatModifiers)
                {
                    position.y += EditorGUIUtility.singleLineHeight + 2;
                    EditorGUI.LabelField(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight),
                        kvp.Key.ToString(), kvp.Value.ToString());
                }
                EditorGUI.indentLevel--;
            }

            // 绘制 Percentage Modifiers
            position.y += EditorGUIUtility.singleLineHeight + 4; // 确保位置向下移动
            Rect percentageModifiersLabelRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(percentageModifiersLabelRect, "Percentage Modifiers");

            if (percentageModifiers != null)
            {
                EditorGUI.indentLevel++;
                foreach (var kvp in percentageModifiers)
                {
                    position.y += EditorGUIUtility.singleLineHeight + 2;
                    EditorGUI.LabelField(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight),
                        kvp.Key.ToString(), kvp.Value.ToString()+"%");
                }
                EditorGUI.indentLevel--;
            }


            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            // 计算属性高度
            object targetObject = GetTargetObjectOfProperty(property);
            FieldInfo flatModifiersField = targetObject.GetType().GetField("flatModifiers", BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo percentageModifiersField = targetObject.GetType().GetField("percentageModifiers", BindingFlags.NonPublic | BindingFlags.Instance);

            var flatModifiers = flatModifiersField?.GetValue(targetObject) as Dictionary<object, float>;
            var percentageModifiers = percentageModifiersField?.GetValue(targetObject) as Dictionary<object, float>;

            int flatModifiersCount = flatModifiers?.Count ?? 0;
            int percentageModifiersCount = percentageModifiers?.Count ?? 0;

            // 基础高度 + 每个字典项的高度
            return EditorGUIUtility.singleLineHeight * (6 + flatModifiersCount + percentageModifiersCount) + 10;
        }

        private object GetTargetObjectOfProperty(SerializedProperty prop)
        {
            if (prop == null) return null;

            string path = prop.propertyPath.Replace(".Array.data[", "[");
            object obj = prop.serializedObject.targetObject;
            string[] elements = path.Split('.');

            foreach (var element in elements)
            {
                if (element.Contains("["))
                {
                    string elementName = element.Substring(0, element.IndexOf("["));
                    int index = System.Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
                    obj = GetValue(obj, elementName, index);
                }
                else
                {
                    obj = GetValue(obj, element);
                }
            }
            return obj;
        }

        private object GetValue(object source, string name)
        {
            if (source == null)
                return null;

            var type = source.GetType();
            var field = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (field == null)
            {
                var property = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (property == null)
                    return null;

                return property.GetValue(source, null);
            }

            return field.GetValue(source);
        }

        private object GetValue(object source, string name, int index)
        {
            var enumerable = GetValue(source, name) as System.Collections.IEnumerable;
            if (enumerable == null) return null;

            var enm = enumerable.GetEnumerator();
            while (index-- >= 0)
                enm.MoveNext();

            return enm.Current;
        }
    }
}