using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Model
{
    [CustomPropertyDrawer(typeof(TypeReference))]
    public class TypeReferenceDrawer : PropertyDrawer
    {
        private static List<Type> cachedTetriCellTypes;
        private static string[] cachedTypeNames;

        static TypeReferenceDrawer()
        {
            CacheTetriCellTypes();
        }

        private static void CacheTetriCellTypes()
        {
            // 缓存所有 Cell 的子类
            cachedTetriCellTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.IsSubclassOf(typeof(Model.Tetri.Cell)) && !type.IsAbstract)
                .ToList();

            // 缓存子类名称
            cachedTypeNames = cachedTetriCellTypes.Select(type => type.Name).ToArray();
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // 获取 typeName 字段
            var typeNameProperty = property.FindPropertyRelative("typeName");

            // 获取当前类型的索引
            var currentType = Type.GetType(typeNameProperty.stringValue);
            int currentIndex = cachedTetriCellTypes.IndexOf(currentType);

            // 构建下拉菜单
            int selectedIndex = EditorGUI.Popup(position, label.text, currentIndex, cachedTypeNames);

            // 更新选中的类型
            if (selectedIndex >= 0 && selectedIndex < cachedTetriCellTypes.Count)
            {
                typeNameProperty.stringValue = cachedTetriCellTypes[selectedIndex].AssemblyQualifiedName;
            }
        }
    }
}