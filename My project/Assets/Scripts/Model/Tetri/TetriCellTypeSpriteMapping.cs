using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


namespace Model.Tetri
{
    [CreateAssetMenu(fileName = "TetriCellTypeSpriteMapping", menuName = "Tetris/TetriCellTypeSpriteMapping")]
    public class TetriCellTypeSpriteMapping : ScriptableObject
    {
        [Serializable]
        public struct CellTypeResourcePair
        {
            public string cellTypeName; // 类类型的名称
            public Sprite sprite;
            public Tile tile;
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
                string fullTypeName = $"Model.Tetri.{pair.cellTypeName}";
                Type cellType = Type.GetType(fullTypeName);

                if (cellType != null)
                {
                    resourceDictionary[cellType] = pair;
                }
                else
                {
                    Debug.LogWarning($"Type {pair.cellTypeName} not found.");
                }
            }
        }

        public Sprite GetSprite(Type cellType)
        {
            if (resourceDictionary.TryGetValue(cellType, out var pair))
            {
                return pair.sprite;
            }
            return null;
        }

        public Tile GetTile(Type cellType)
        {
            if (resourceDictionary.TryGetValue(cellType, out var pair))
            {
                return pair.tile;
            }
            return null;
        }

        // public Sprite GetSprite<T>() where T : TetriCell
        // {
        //     return GetSprite(typeof(T));
        // }

    }
}