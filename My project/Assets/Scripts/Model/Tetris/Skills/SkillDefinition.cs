using System;
using Units.Skills;
using UnityEngine;

namespace Model.Tetri
{
    [CreateAssetMenu(menuName = "Config/Skills/Skill Definition")]
    public sealed class SkillDefinition : ScriptableObject
    {
        [SerializeField] private string id;
        [SerializeField] private string displayName;
        [SerializeField] private string runtimeTypeName;
        [SerializeField] private SkillConfig config;

        public string Id => id;
        public string DisplayName => displayName;
        public string RuntimeTypeName => runtimeTypeName;
        public SkillConfig Config => config;
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
                id = ResolveRuntimeType()?.Name;
            }
            catch
            {
                // Ignore validation errors until the runtime type is fixed.
            }
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

            if (!typeof(Skill).IsAssignableFrom(resolvedType))
            {
                throw new InvalidOperationException($"Resolved type '{resolvedType.FullName}' is not assignable to {nameof(Skill)}. Definition={name}");
            }

            return resolvedType;
        }
    }
}
