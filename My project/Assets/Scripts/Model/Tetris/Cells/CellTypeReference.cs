using System;
using Model.Tetri;

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

        internal Cell CreateInstance()
        {
            return (Cell)Activator.CreateInstance(Type);
        }
    }
}