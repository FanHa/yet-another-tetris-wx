using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Model.Tetri.Editor
{
    public static class CharacterDefinitionAssetCreator
    {
    private const string RootPath = "Assets/Data/Gameplay/Cells/Definitions/Characters";

        [MenuItem("Tools/Tetris/Create Character Definitions")]
        public static void CreateAll()
        {
            EnsureFolder();

            var definitions = new[]
            {
                new CharacterSpec("Square", "Square", "稳扎稳打的基础角色，擅长在前排撑住局势。", 2.2f, 12f, 120f, 2.5f, 5f, 0.2f),
                new CharacterSpec("Triangle", "Triangle", "以高爆发和快速压制著称，适合打击关键目标。", 2.8f, 15f, 100f, 2.8f, 5.5f, 0.25f),
                new CharacterSpec("Circle", "Circle", "高生命和持续输出的耐久型角色，适合站位和拖延。", 2.0f, 9f, 130f, 2.2f, 4.5f, 0.18f),
                new CharacterSpec("Aim", "Aim", "精准度极高，擅长远程打击和补刀。", 2.5f, 13f, 110f, 3.0f, 6.0f, 0.3f),
                new CharacterSpec("Hourglass", "Hourglass", "节奏控制型角色，擅长在关键时刻扭转战局。", 1.8f, 14f, 140f, 2.0f, 5.2f, 0.22f)
            };

            foreach (var spec in definitions)
            {
                CreateOrUpdate(spec);
            }

            RegisterToDatabase(definitions);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Character definition assets created and registered.");
        }

        private static void EnsureFolder()
        {
            if (!AssetDatabase.IsValidFolder(RootPath))
            {
                 var parent = "Assets/Data/Gameplay/Cells/Definitions";
                AssetDatabase.CreateFolder(parent, "Characters");
            }
        }

        private static void CreateOrUpdate(CharacterSpec spec)
        {
            var assetPath = $"{RootPath}/{spec.Name}.asset";
            var asset = AssetDatabase.LoadAssetAtPath<CharacterDefinition>(assetPath);

            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<CharacterDefinition>();
                AssetDatabase.CreateAsset(asset, assetPath);
            }

            var so = new SerializedObject(asset);
            SetString(so, "id", spec.Id);
            SetEnum(so, "affinity", AffinityType.None);
            SetString(so, "displayName", spec.DisplayName);
            SetString(so, "description", spec.Description);
            SetFloat(so, "moveSpeedBase", spec.MoveSpeedBase);
            SetFloat(so, "attackPowerBase", spec.AttackPowerBase);
            SetFloat(so, "maxHealthBase", spec.MaxHealthBase);
            SetFloat(so, "attacksPerTenSecondsBase", spec.AttacksPerTenSecondsBase);
            SetFloat(so, "energyPerSecondBase", spec.EnergyPerSecondBase);
            SetFloat(so, "attackRangeBase", spec.AttackRangeBase);
            SetObjectReference(so, "icon", null);

            so.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(asset);
        }

        private static void RegisterToDatabase(IEnumerable<CharacterSpec> definitions)
        {
              var databasePath = "Assets/Data/Gameplay/Cells/CellDatabase.asset";
            var database = AssetDatabase.LoadAssetAtPath<CellDatabase>(databasePath);
            if (database == null)
            {
                throw new InvalidOperationException($"CellDatabase not found at {databasePath}");
            }

            var dbSo = new SerializedObject(database);
            var list = dbSo.FindProperty("registeredCellDefinitions");
            if (list == null)
            {
                throw new InvalidOperationException("registeredCellDefinitions not found on CellDatabase.");
            }

            var existing = new HashSet<string>();
            for (int i = 0; i < list.arraySize; i++)
            {
                var obj = list.GetArrayElementAtIndex(i).objectReferenceValue as CharacterDefinition;
                if (obj != null)
                {
                    existing.Add(obj.Id);
                }
            }

            foreach (var spec in definitions)
            {
                var assetPath = $"{RootPath}/{spec.Name}.asset";
                var asset = AssetDatabase.LoadAssetAtPath<CharacterDefinition>(assetPath);
                if (asset == null)
                {
                    continue;
                }

                if (existing.Contains(asset.Id))
                {
                    continue;
                }

                list.arraySize += 1;
                list.GetArrayElementAtIndex(list.arraySize - 1).objectReferenceValue = asset;
                existing.Add(asset.Id);
            }

            dbSo.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(database);
        }

        private static void SetString(SerializedObject so, string fieldName, string value)
        {
            var prop = so.FindProperty(fieldName);
            if (prop == null || prop.propertyType != SerializedPropertyType.String)
            {
                return;
            }

            prop.stringValue = value;
        }

        private static void SetEnum(SerializedObject so, string fieldName, AffinityType value)
        {
            var prop = so.FindProperty(fieldName);
            if (prop == null || prop.propertyType != SerializedPropertyType.Enum)
            {
                return;
            }

            prop.enumValueIndex = (int)value;
        }

        private static void SetFloat(SerializedObject so, string fieldName, float value)
        {
            var prop = so.FindProperty(fieldName);
            if (prop == null || prop.propertyType != SerializedPropertyType.Float)
            {
                return;
            }

            prop.floatValue = value;
        }

        private static void SetObjectReference(SerializedObject so, string fieldName, UnityEngine.Object value)
        {
            var prop = so.FindProperty(fieldName);
            if (prop == null || prop.propertyType != SerializedPropertyType.ObjectReference)
            {
                return;
            }

            prop.objectReferenceValue = value;
        }

        private struct CharacterSpec
        {
            public string Name;
            public string Id;
            public string DisplayName;
            public string Description;
            public float MoveSpeedBase;
            public float AttackPowerBase;
            public float MaxHealthBase;
            public float AttacksPerTenSecondsBase;
            public float EnergyPerSecondBase;
            public float AttackRangeBase;

            public CharacterSpec(string name, string id, string description, float moveSpeedBase, float attackPowerBase, float maxHealthBase, float attacksPerTenSecondsBase, float energyPerSecondBase, float attackRangeBase)
            {
                Name = name;
                Id = id;
                DisplayName = id;
                Description = description;
                MoveSpeedBase = moveSpeedBase;
                AttackPowerBase = attackPowerBase;
                MaxHealthBase = maxHealthBase;
                AttacksPerTenSecondsBase = attacksPerTenSecondsBase;
                EnergyPerSecondBase = energyPerSecondBase;
                AttackRangeBase = attackRangeBase;
            }
        }
    }
}
