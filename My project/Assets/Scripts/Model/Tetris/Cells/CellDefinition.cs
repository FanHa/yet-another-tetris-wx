using System;
using UnityEngine;

namespace Model.Tetri
{
    public abstract class CellDefinition : ScriptableObject
    {
        [SerializeField] private string id;
        [SerializeField] private string runtimeTypeName;

        public string Id => ResolveRuntimeType().Name;
        public string RuntimeTypeName => runtimeTypeName;
        public abstract Sprite Icon { get; }
        public Type RuntimeType => ResolveRuntimeType();

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (string.IsNullOrWhiteSpace(runtimeTypeName))
            {
                return;
            }

            try
            {
                id = ResolveRuntimeType().Name;
            }
            catch
            {
                // Keep editing experience smooth while runtime type is being fixed.
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
