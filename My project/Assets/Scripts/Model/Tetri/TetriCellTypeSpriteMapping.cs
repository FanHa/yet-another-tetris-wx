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

        // 新增的重载方法，接受 TetriCell 对象作为参数
        public Sprite GetSprite(TetriCell cell)
        {
            if (cell == null) return null;
            return GetSprite(cell.GetType());
        }

        public Tile GetTile(TetriCell cell)
        {
            if (cell == null) return null;
            return GetTile(cell.GetType());
        }

    }
}