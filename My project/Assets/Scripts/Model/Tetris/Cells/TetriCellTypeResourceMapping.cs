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
            public string characterTypeId;
            public Sprite sprite;
        }

        [SerializeField]
        private List<CharacterTypeResourcePair> characterMappings;

        private Dictionary<string, Sprite> characterResourceDictionary;

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
            characterResourceDictionary = new Dictionary<string, Sprite>(StringComparer.Ordinal);
            foreach (var pair in characterMappings)
            {
                if (string.IsNullOrWhiteSpace(pair.characterTypeId))
                    continue;

                characterResourceDictionary[pair.characterTypeId] = pair.sprite;
            }
        }

        public Sprite GetSprite(string characterTypeId)
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
            return null;
        }
    }
}