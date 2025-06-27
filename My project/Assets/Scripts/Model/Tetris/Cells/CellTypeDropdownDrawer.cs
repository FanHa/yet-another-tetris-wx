using UnityEditor;
using UnityEngine;
using System.Linq;

[CustomPropertyDrawer(typeof(CellTypeDropdownAttribute))]
public class CellTypeDropdownDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // 获取 TetriCellFactory 实例
        var factory = property.serializedObject.targetObject as Model.Tetri.TetriCellFactory;
        if (factory == null)
        {
            EditorGUI.PropertyField(position, property, label);
            return;
        }

        var typeList = factory.AvailableCellTypes;
        var typeNames = typeList.Select(t => t.FullName).ToList();
        var displayNames = typeList.Select(t => t.Name).ToArray();

        int index = Mathf.Max(0, typeNames.IndexOf(property.stringValue));
        int newIndex = EditorGUI.Popup(position, label.text, index, displayNames);

        if (newIndex >= 0 && newIndex < typeNames.Count)
            property.stringValue = typeNames[newIndex];
    }
}