using System.Text.RegularExpressions;
using UnityEngine;
using Units;

namespace Model.Buffs
{
    public abstract class BuffDefinition : ScriptableObject
    {
        [SerializeField] private string id;
        [SerializeField] private string displayName;
        [TextArea, SerializeField] private string description;
        [SerializeField] private Sprite icon;
        [SerializeField] private BuffVisual visualPrefab;
        [SerializeField] private BuffDuration baseDuration = BuffDuration.Permanent;

        public string Id => id;
        public string DisplayName => displayName;
        public string Description => description;
        public Sprite Icon => icon;
        public BuffVisual VisualPrefab => visualPrefab;
        public BuffDuration BaseDuration => baseDuration;

#if UNITY_EDITOR
        private static readonly Regex IdPattern = new("^buff\\.[a-z][a-z0-9]*(?:_[a-z0-9]+)*$", RegexOptions.Compiled);

        protected virtual void OnValidate()
        {
            if (string.IsNullOrWhiteSpace(displayName))
            {
                Debug.LogError($"{GetType().Name} '{name}'.{nameof(DisplayName)} is not assigned.", this);
            }

            if (string.IsNullOrWhiteSpace(description))
            {
                Debug.LogError($"{GetType().Name} '{name}'.{nameof(Description)} is not assigned.", this);
            }

            if (icon == null)
            {
                Debug.LogError($"{GetType().Name} '{name}'.{nameof(Icon)} is not assigned.", this);
            }

            if (!string.IsNullOrWhiteSpace(id) && !IdPattern.IsMatch(id))
            {
                Debug.LogError($"{nameof(id)} '{id}' is invalid. Expected the format 'buff.example_name'.", this);
            }

            if (!baseDuration.IsValid())
            {
                Debug.LogError($"{GetType().Name} '{name}'.{nameof(BaseDuration)} must be permanent or have a finite duration greater than zero.", this);
            }
        }

        protected void ValidateTimedBaseDuration()
        {
            if (!baseDuration.IsTimed)
            {
                Debug.LogError($"{GetType().Name} '{name}'.{nameof(BaseDuration)} must be finite.", this);
            }
        }
#endif
    }
}