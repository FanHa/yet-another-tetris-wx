using System;
using System.Text.RegularExpressions;
using Model.Resonance;
using UnityEngine;

namespace Model.Tetri
{
    public abstract class CellDefinition : ScriptableObject
    {
        [SerializeField] private string id;
        [SerializeField] private ResonanceType resonance = ResonanceType.None;

        public string Id => id;
        public ResonanceType Resonance => resonance;
        public abstract Sprite Icon { get; }
        public virtual string DisplayName => null;
        public virtual string Description => null;

#if UNITY_EDITOR
        private static readonly Regex IdPattern = new("^[A-Za-z][A-Za-z0-9_]*$", RegexOptions.Compiled);

        protected virtual void OnValidate()
        {
            if (!Enum.IsDefined(typeof(ResonanceType), resonance))
            {
                Debug.LogError($"{nameof(resonance)} '{resonance}' is invalid.", this);
            }

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
