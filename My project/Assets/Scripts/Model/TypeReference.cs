using System;

namespace Model
{
    [Serializable]
    public class TypeReference
    {
        public string typeName;

        public Type Type
        {
            get => Type.GetType(typeName);
            set => typeName = value?.AssemblyQualifiedName;
        }
    }
}