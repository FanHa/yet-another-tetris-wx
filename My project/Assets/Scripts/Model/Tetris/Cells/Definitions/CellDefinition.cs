using System;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Model.Tetri
{
    public abstract class CellDefinition : ScriptableObject
    {
        [SerializeField] private string id;
        [SerializeField] private AffinityType affinity = AffinityType.None;

        public string Id => id;
        public AffinityType Affinity => affinity;
        public abstract Sprite Icon { get; }
        public virtual string DisplayName => null;
        public virtual string Description => null;

#if UNITY_EDITOR
        private static readonly Regex IdPattern = new("^[A-Za-z][A-Za-z0-9_]*$", RegexOptions.Compiled);

        private void OnValidate()
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                id = name;
            }

            if (!string.Equals(id, id.Trim(), StringComparison.Ordinal) || !IdPattern.IsMatch(id))
            {
                Debug.LogError($"CellDefinition ID '{id}' is invalid. Expected pattern: {IdPattern}.", this);
            }
        }
#endif
    }
}
