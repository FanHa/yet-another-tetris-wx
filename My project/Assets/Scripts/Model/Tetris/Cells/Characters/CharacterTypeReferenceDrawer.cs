using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Model
{
    [CustomPropertyDrawer(typeof(CharacterTypeReference))]
    public class CharacterTypeReferenceDrawer : PropertyDrawer
    {
        private static List<Type> cachedCharacterTypes;
        private static string[] cachedTypeNames;

        static CharacterTypeReferenceDrawer()
        {
            CacheCharacterTypes();
        }

        private static void CacheCharacterTypes()
        {
            cachedCharacterTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.IsSubclassOf(typeof(Model.Tetri.Character)) && !type.IsAbstract)
                .ToList();

            cachedTypeNames = cachedCharacterTypes.Select(type => type.Name).ToArray();
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var typeNameProperty = property.FindPropertyRelative("typeName");
            var currentType = Type.GetType(typeNameProperty.stringValue);
            int currentIndex = cachedCharacterTypes.IndexOf(currentType);

            int selectedIndex = EditorGUI.Popup(position, label.text, currentIndex, cachedTypeNames);

            if (selectedIndex >= 0 && selectedIndex < cachedCharacterTypes.Count)
            {
                typeNameProperty.stringValue = cachedCharacterTypes[selectedIndex].AssemblyQualifiedName;
            }
        }
    }
}
