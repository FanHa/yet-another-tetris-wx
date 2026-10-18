using UnityEngine;

namespace Model.Tetri
{
    [CreateAssetMenu(menuName = "Game Data/Bootstrap/Factories/Character Factory")]
    public sealed class CharacterFactory : ScriptableObject
    {
        [SerializeField] private CharacterDatabase characterDatabase;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (characterDatabase == null)
            {
                Debug.LogError($"[{nameof(CharacterFactory)}] Character database is missing.", this);
            }
        }
#endif

        public Character CreateCharacter(string definitionId)
        {
            return CreateCharacter(characterDatabase.GetDefinition(definitionId));
        }

        public Character CreateCharacter(CharacterDefinition definition)
        {
            var character = new Character();
            character.Initialize(definition);
            return character;
        }

        public Character Clone(Character character)
        {
            return character.Clone();
        }
    }
}