using System;
using Units.Skills;
using UnityEngine;

namespace Model.Tetri
{
    [CreateAssetMenu(menuName = "Config/Cells/Cell Definition")]
    public sealed class CellDefinition : ScriptableObject
    {
        [SerializeField] private string id;
        [SerializeField] private string runtimeTypeName;
        [SerializeField] private Sprite icon;

        public string Id => RuntimeType.Name;
        public string RuntimeTypeName => runtimeTypeName;
        public Sprite Icon => icon;
        public Type RuntimeType => ResolveRuntimeType();

#if UNITY_EDITOR
        private void OnValidate()
        {
            id = RuntimeType.Name;
        }
#endif

        private Type ResolveRuntimeType()
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
