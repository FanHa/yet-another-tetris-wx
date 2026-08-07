using System;
using UnityEngine;

namespace Model.Tetri
{
    public abstract class CellDefinition : ScriptableObject
    {
        [SerializeField] private string id;
        [SerializeField] private string runtimeTypeName;
        [SerializeField] private AffinityType affinity = AffinityType.None;

        public string Id => id;
        public string RuntimeTypeName => runtimeTypeName;
        public AffinityType Affinity => affinity;
        public abstract Sprite Icon { get; }
        public Type RuntimeType => ResolveRuntimeType();

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                // Keep id stable and explicit instead of coupling it to runtime type.
                id = name;
            }
        }
#endif

        protected Type ResolveRuntimeType()
        {
            if (string.IsNullOrWhiteSpace(runtimeTypeName))
            {
                throw new InvalidOperationException($"RuntimeTypeName is empty. Definition={name}");
            }

            Type resolvedType = Type.GetType(runtimeTypeName, false);
            if (resolvedType == null)
            {
                throw new InvalidOperationException($"Cannot resolve RuntimeTypeName '{runtimeTypeName}'. Definition={name}");
            }

            if (!typeof(Cell).IsAssignableFrom(resolvedType))
            {
                throw new InvalidOperationException($"Resolved type '{resolvedType.FullName}' is not assignable to {nameof(Cell)}. Definition={name}");
            }

            return resolvedType;
        }
    }
}
