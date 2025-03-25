using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Units;


namespace Model.Tetri
{
    [CreateAssetMenu(fileName = "TetriCellTypeResourceMapping", menuName = "Tetris/TetriCellTypeResourceMapping")]
    public class TetriCellTypeResourceMapping : ScriptableObject
    {
        [Serializable]
        public struct CellTypeResourcePair
        {
            // todo 做成选项
            public Model.TypeReference cellType; // 类类型的名称
            public Sprite sprite;
            public GameObject prefab;
        }

        [SerializeField]
        private List<CellTypeResourcePair> mappings;

        public List<CellTypeResourcePair> Mappings => mappings;

        private Dictionary<Type, CellTypeResourcePair> resourceDictionary;


        private void OnEnable()
        {
            InitializeDictionary();
        }

        private void InitializeDictionary()
        {
            resourceDictionary = new Dictionary<Type, CellTypeResourcePair>();
            foreach (var pair in mappings)
            {
                if (pair.cellType.Type != null)
                {
                    resourceDictionary[pair.cellType.Type] = pair;
                }
                else
                {
                    Debug.LogWarning("Invalid type in CellTypeResourcePair.");
                }
            }
        }

        public Sprite GetSprite(TetriCell cell)
        {
            if (cell == null) return null;
            return GetSprite(cell.GetType());
        }

        public Sprite GetSprite(Type type)
        {
            if (resourceDictionary.TryGetValue(type, out var pair))
            {
                return pair.sprite;
            }
            return null;
        }


        public GameObject GetPrefab(TetriCell cell)
        {
            if (cell == null) return null;
            return GetPrefab(cell.GetType());
        }

        public GameObject GetPrefab(Type type)
        {
            if (resourceDictionary.TryGetValue(type, out var pair))
            {
                return pair.prefab;
            }
            return null;
        }

        
    }
}