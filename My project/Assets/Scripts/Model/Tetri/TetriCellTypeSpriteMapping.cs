using System;
using System.Collections.Generic;
using UnityEngine;

namespace Model.Tetri
{
    [CreateAssetMenu(fileName = "TetriCellTypeSpriteMapping", menuName = "Tetris/TetriCellTypeSpriteMapping")]
    public class TetriCellTypeSpriteMapping : ScriptableObject
    {
        [Serializable]
        public struct CellTypeSpritePair
        {
            public string cellTypeName; // 类类型的名称
            public Sprite sprite;
        }

        [SerializeField]
        private List<CellTypeSpritePair> mappings;

        public List<CellTypeSpritePair> Mappings => mappings;

        private Dictionary<Type, Sprite> spriteDictionary;

        private void OnEnable()
        {
            InitializeDictionary();
        }

        private void InitializeDictionary()
        {
            spriteDictionary = new Dictionary<Type, Sprite>();
            foreach (var pair in mappings)
            {
                string fullTypeName = $"Model.Tetri.{pair.cellTypeName}";
                Type cellType = Type.GetType(fullTypeName);

                if (cellType != null)
                {
                    spriteDictionary[cellType] = pair.sprite;
                }
                else
                {
                    Debug.LogWarning($"Type {pair.cellTypeName} not found.");
                }
            }
        }

        public Sprite GetSprite(Type cellType)
        {
            if (spriteDictionary.TryGetValue(cellType, out var sprite))
            {
                return sprite;
            }
            return null;
        }

        public Sprite GetSprite<T>() where T : TetriCell
        {
            return GetSprite(typeof(T));
        }

    }
}