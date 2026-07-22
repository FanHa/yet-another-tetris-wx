using System;
using System.Collections.Generic;
using UnityEngine;

namespace Model.Tetri
{
    [CreateAssetMenu(fileName = "TetriCellTypeResourceMapping", menuName = "Tetris/TetriCellTypeResourceMapping")]
    public class TetriCellTypeResourceMapping : ScriptableObject
    {
        [Serializable]
        public struct CharacterTypeResourcePair
        {
            public CharacterTypeId characterTypeId;
            public Sprite sprite;
        }

        [SerializeField]
        private List<CharacterTypeResourcePair> characterMappings;

        private Dictionary<CharacterTypeId, Sprite> characterResourceDictionary;

        [SerializeField] private Model.Tetri.TetriCellFactory tetriCellFactory;

        private void OnEnable()
        {
            InitializeDictionary();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            InitializeDictionary();
        }
#endif

        private void InitializeDictionary()
        {
            characterResourceDictionary = new Dictionary<CharacterTypeId, Sprite>();
            foreach (var pair in characterMappings)
            {
                characterResourceDictionary[pair.characterTypeId] = pair.sprite;
            }
        }

        public Sprite GetSprite(CharacterTypeId characterTypeId)
        {
            if (characterResourceDictionary == null)
            {
                InitializeDictionary();
            }

            if (characterResourceDictionary.TryGetValue(characterTypeId, out var sprite))
            {
                return sprite;
            }
            return null;
        }

        public Sprite GetSprite(Type type)
        {
            if (tetriCellFactory.TypeToCharacterTypeId != null &&
                tetriCellFactory.TypeToCharacterTypeId.TryGetValue(type, out var characterTypeId))
            {
                return GetSprite(characterTypeId);
            }
            return null;
        }
    }
}