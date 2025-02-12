using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEngine;
using Model;
using Model.Tetri;

namespace Model {
    [CustomPropertyDrawer(typeof(Serializable2DArray<>))]
    public class Serializable2DArrayDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var container = new VisualElement();

            // Draw label
            container.Add(new Label(property.displayName));

            int rows = property.FindPropertyRelative("rows").intValue;
            int cols = property.FindPropertyRelative("cols").intValue;

            var rowsField = new IntegerField("Rows") { value = rows };
            var colsField = new IntegerField("Cols") { value = cols };
            container.Add(rowsField);
            container.Add(colsField);

            SerializedProperty arrayProperty = property.FindPropertyRelative("array");

            // Draw array elements
            for (int i = 0; i < rows; i++)
            {
                var rowContainer = new VisualElement();
                rowContainer.style.flexDirection = FlexDirection.Row;
                for (int j = 0; j < cols; j++)
                {
                    int index = i * cols + j;
                    SerializedProperty element = arrayProperty.GetArrayElementAtIndex(index);

                    if (element != null)
                    {
                        SerializedProperty nameProperty = element.FindPropertyRelative("Name");
                        string name = nameProperty != null ? nameProperty.stringValue : "No Name";
                        rowContainer.Add(new Label(name));
                    }
                    else
                    {
                        rowContainer.Add(new Label("Null"));
                    }
                }
                container.Add(rowContainer);
            }

            return container;
        }
        // public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        // {
            
        //     // EditorGUI.BeginProperty(position, label, property);

        //     // // Draw label
        //     // EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        //     // // Don't make child fields be indented
        //     // var indent = EditorGUI.indentLevel;
        //     // EditorGUI.indentLevel = 0;
        //     EditorGUILayout.LabelField(label);
        //     var arrayRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width , position.height);
        //     int rows = property.FindPropertyRelative("rows").intValue;
        //     int cols = property.FindPropertyRelative("cols").intValue;
        //     EditorGUILayout.BeginHorizontal();
        //     EditorGUILayout.LabelField("Rows", GUILayout.MaxWidth(50));
        //     EditorGUILayout.IntField(rows);
        //     EditorGUILayout.LabelField("Cols", GUILayout.MaxWidth(50));
        //     EditorGUILayout.IntField(cols);
        //     EditorGUILayout.EndHorizontal();

            
        //     SerializedProperty arrayProperty = property.FindPropertyRelative("array");

        //     // // Draw array elements
        //     float cellWidth = position.width / cols;
        //     float cellHeight = EditorGUIUtility.singleLineHeight;
        //     for (int i = 0; i < rows; i++)
        //     {
        //         EditorGUILayout.BeginHorizontal();
        //         for (int j = 0; j < cols; j++)
        //         {
        //             int index = i * cols + j;
        //             SerializedProperty element = arrayProperty.GetArrayElementAtIndex(index);
                    
        //             if (element != null)
        //             {
        //                 SerializedProperty nameProperty = element.FindPropertyRelative("Name");
        //                 string name = nameProperty != null ? nameProperty.stringValue : "No Name";
        //                 EditorGUILayout.LabelField(name, GUILayout.Width(cellWidth));
        //             }
        //             else
        //             {
        //                 EditorGUILayout.LabelField("Null", GUILayout.Width(cellWidth));
        //             }
        //         }
        //         EditorGUILayout.EndHorizontal();
        //     }
        //     // // Set indent back to what it was
        //     // // EditorGUI.indentLevel = indent;

        //     // EditorGUI.EndProperty();
        // }
    }
}