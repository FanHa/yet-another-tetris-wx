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
        [SerializeField] private SkillConfig config;
        [SerializeField] private AffinityType affinity = AffinityType.None;
        [SerializeField] private string displayName;
        [SerializeField] [TextArea] private string description;
        [SerializeField] private Sprite icon;
        [SerializeField] private bool isHidden;
        [SerializeField] private bool isDebugOnly;

        public string Id => id;
        public string RuntimeTypeName => runtimeTypeName;
        public SkillConfig Config => config;
        public AffinityType Affinity => affinity;
        public string DisplayName => displayName;
        public string Description => description;
        public Sprite Icon => icon;
        public bool IsHidden => isHidden;
        public bool IsDebugOnly => isDebugOnly;

        public bool TryResolveCellType(out Type cellType, out string error)
        {
            cellType = null;
            error = null;

            if (string.IsNullOrWhiteSpace(runtimeTypeName))
            {
                error = $"RuntimeTypeName is empty. Definition={name}";
                return false;
            }

            Type resolvedType = Type.GetType(runtimeTypeName, false);
            if (resolvedType == null)
            {
                error = $"Cannot resolve RuntimeTypeName '{runtimeTypeName}'. Definition={name}";
                return false;
            }

            if (!typeof(Cell).IsAssignableFrom(resolvedType))
            {
                error = $"Resolved type '{resolvedType.FullName}' is not assignable to {nameof(Cell)}. Definition={name}";
                return false;
            }

            cellType = resolvedType;
            return true;
        }
    }
}
