using System;
using System.Collections.Generic;
using UnityEngine;

namespace Model.Tetri
{
    [CreateAssetMenu(fileName = "CharacterTypePrefabMapping", menuName = "Tetris/CharacterTypePrefabMapping")]
    public class CharacterTypePrefabMapping : ScriptableObject
    {
        [Serializable]
        public struct CellPrefabPair
        {
            public Model.CharacterTypeReference characterType; // Class type reference
            public GameObject prefab; // Prefab associated with the cell type
        }

        [SerializeField]
        private List<CellPrefabPair> mappings;

        public List<CellPrefabPair> Mappings => mappings;

        private Dictionary<Type, GameObject> prefabDictionary;

        private void OnEnable()
        {
            InitializeDictionary();
        }

        private void InitializeDictionary()
        {
            prefabDictionary = new Dictionary<Type, GameObject>();
            foreach (var pair in mappings)
            {
                if (pair.characterType.Type != null)
                {
                    prefabDictionary[pair.characterType.Type] = pair.prefab;
                }
                else
                {
                    Debug.LogWarning("Invalid type in CellPrefabPair.");
                }
            }
        }

        public GameObject GetPrefab(Cell cell)
        {
            if (cell == null) return null;
            return GetPrefab(cell.GetType());
        }

        public GameObject GetPrefab(Type type)
        {
            if (prefabDictionary.TryGetValue(type, out var prefab))
            {
                return prefab;
            }
            return null;
        }
    }
}
