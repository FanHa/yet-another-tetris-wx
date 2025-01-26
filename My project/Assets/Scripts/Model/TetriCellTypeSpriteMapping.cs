using System;
using System.Collections.Generic;
using UnityEngine;

namespace Model
{
    [CreateAssetMenu(fileName = "TetriCellTypeSpriteMapping", menuName = "Tetris/TetriCellTypeSpriteMapping")]
    public class TetriCellTypeSpriteMapping : ScriptableObject
    {
        [Serializable]
        public struct CellTypeSpritePair
        {
            public TetriCell.CellType cellType;
            public Sprite sprite;
        }

        [SerializeField]
        private List<CellTypeSpritePair> mappings;

        public List<CellTypeSpritePair> Mappings => mappings;

        private Dictionary<TetriCell.CellType, Sprite> spriteDictionary;

        private void OnEnable()
        {
            InitializeDictionary();
        }

        private void InitializeDictionary()
        {
            spriteDictionary = new Dictionary<TetriCell.CellType, Sprite>();
            foreach (var pair in mappings)
            {
                spriteDictionary[pair.cellType] = pair.sprite;
            }
        }

        public Sprite GetSprite(TetriCell.CellType type)
        {
            if (spriteDictionary.TryGetValue(type, out var sprite))
            {
                return sprite;
            }
            return null;
        }

        // private void OnValidate()
        // {
        //     // 确保每个 CellType 都有对应的 Sprite
        //     var cellTypes = Enum.GetValues(typeof(TetriCell.CellType));
        //     foreach (TetriCell.CellType cellType in cellTypes)
        //     {
        //         bool found = false;
        //         foreach (var pair in mappings)
        //         {
        //             if (pair.cellType == cellType)
        //             {
        //                 found = true;
        //                 break;
        //             }
        //         }
        //         if (!found)
        //         {
        //             Debug.LogWarning($"CellType {cellType} does not have a corresponding Sprite.");
        //         }
        //     }

        //     // 重新初始化字典
        //     InitializeDictionary();
        // }
    }
}