using System;

namespace Model
{
    [Serializable]
    public class CharacterTypeReference
    {
        public string typeName;

        public Type Type
        {
            get => Type.GetType(typeName);
            set => typeName = value?.AssemblyQualifiedName;
        }

        // public bool IsCharacterType => Type != null && Type.IsSubclassOf(typeof(Model.Tetri.Character));
    }
}
