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
            public CellTypeId cellTypeId; // 直接用枚举
            public Sprite sprite;
        }

        [Serializable]
        public struct CharacterTypeResourcePair
        {
            public CharacterTypeId characterTypeId;
            public Sprite sprite;
        }


        [SerializeField]
        private List<CellTypeResourcePair> cellMappings;
        [SerializeField]
        private List<CharacterTypeResourcePair> characterMappings;

        private Dictionary<CellTypeId, Sprite> cellResourceDictionary;
        private Dictionary<CharacterTypeId, Sprite> characterResourceDictionary;

        [SerializeField] private Model.Tetri.TetriCellFactory tetriCellFactory;

        private void OnEnable()
        {
            InitializeDictionary();
        }

        private void InitializeDictionary()
        {
            cellResourceDictionary = new Dictionary<CellTypeId, Sprite>();
            foreach (var pair in cellMappings)
            {
                cellResourceDictionary[pair.cellTypeId] = pair.sprite;
            }

            characterResourceDictionary = new Dictionary<CharacterTypeId, Sprite>();
            foreach (var pair in characterMappings)
            {
                characterResourceDictionary[pair.characterTypeId] = pair.sprite;
            }
        }

        public Sprite GetSprite(CellTypeId cellTypeId)
        {
            if (cellResourceDictionary.TryGetValue(cellTypeId, out var sprite))
            {
                return sprite;
            }
            return null;
        }

        public Sprite GetSprite(CharacterTypeId characterTypeId)
        {
            if (characterResourceDictionary.TryGetValue(characterTypeId, out var sprite))
            {
                return sprite;
            }
            return null;
        }

        public Sprite GetSprite(Type type)
        {
            if (tetriCellFactory.TypeToCellTypeId.TryGetValue(type, out var cellTypeId))
            {
                return GetSprite(cellTypeId);
            }
            if (tetriCellFactory.TypeToCharacterTypeId != null &&
                tetriCellFactory.TypeToCharacterTypeId.TryGetValue(type, out var characterTypeId))
            {
                return GetSprite(characterTypeId);
            }
            return null;
        }

        public Sprite GetSprite(Model.Tetri.Cell cell)
        {
            return GetSprite(cell.GetType());
        }
    }
}