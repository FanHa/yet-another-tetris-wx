using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Model
{
    [CustomPropertyDrawer(typeof(CellTypeReference))]
    public class TypeReferenceDrawer : PropertyDrawer
    {
        private  List<Type> cachedCellTypes;
        private  string[] cachedTypeNames;

        private void CacheCellTypes()
        {
            cachedCellTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.IsSubclassOf(typeof(Model.Tetri.Cell)) && !type.IsAbstract)
                .ToList();

            cachedTypeNames = cachedCellTypes.Select(type => type.FullName).ToArray();
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (cachedCellTypes == null || cachedTypeNames == null || cachedCellTypes.Count == 0)
            {
                CacheCellTypes();
            }
            var typeNameProperty = property.FindPropertyRelative("typeName");
            var currentType = Type.GetType(typeNameProperty.stringValue);
            int currentIndex = cachedCellTypes.IndexOf(currentType);

            int selectedIndex = EditorGUI.Popup(position, label.text, currentIndex, cachedTypeNames);

            if (selectedIndex >= 0 && selectedIndex < cachedCellTypes.Count)
            {
                typeNameProperty.stringValue = cachedCellTypes[selectedIndex].AssemblyQualifiedName;
            }
        }
    }
}