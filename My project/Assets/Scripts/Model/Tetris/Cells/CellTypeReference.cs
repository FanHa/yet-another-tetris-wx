using System;

namespace Model
{
    [Serializable]
    public class CellTypeReference
    {
        public string typeName;

        public Type Type
        {
            get => Type.GetType(typeName);
            set => typeName = value?.AssemblyQualifiedName;
        }

        // public bool IsCellType => Type != null && Type.IsSubclassOf(typeof(Model.Tetri.Cell));
    }
}