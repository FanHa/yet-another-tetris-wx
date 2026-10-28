using System.Text.RegularExpressions;
using Model.Resonance;
using UnityEngine;

namespace Model.Skills
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Skills/Definitions/Skill Definition")]
    public class SkillDefinition : ScriptableObject
    {
        [SerializeField] private string id;
        [SerializeField] private string displayName;
        [SerializeField] private string description;
        [SerializeField] private ResonanceType resonance = ResonanceType.None;
        [SerializeField] private Sprite icon;

#if UNITY_EDITOR
        private static readonly Regex IdPattern = new("^skill\\.[a-z][a-z0-9]*(?:_[a-z0-9]+)*$", RegexOptions.Compiled);
#endif
        public string Id => id;
        public string DisplayName => displayName;
        public string Description => description;
        public ResonanceType Resonance => resonance;
        public Sprite Icon => icon;

    #if UNITY_EDITOR
        // TODO: Share this definition configuration across levels; add level-specific configuration only when required.
        protected virtual void OnValidate()
        {
            if (!ValidateDefinitionFields())
            {
                return;
            }

        }

        protected bool ValidateDefinitionFields()
        {
            if (string.IsNullOrWhiteSpace(displayName))
            {
                Debug.LogError($"{nameof(displayName)} is not assigned.", this);
            }

            if (string.IsNullOrWhiteSpace(description))
            {
                Debug.LogError($"{nameof(description)} is not assigned.", this);
            }

            if (icon == null)
            {
                Debug.LogError($"{nameof(icon)} is not assigned.", this);
            }

            if (!System.Enum.IsDefined(typeof(ResonanceType), resonance))
            {
                Debug.LogError($"{nameof(resonance)} '{resonance}' is invalid.", this);
            }

            if (string.IsNullOrWhiteSpace(id))
            {
                Debug.LogError($"{nameof(id)} is not assigned. Expected the format 'skill.example_name'.", this);
                return false;
            }

            if (!IdPattern.IsMatch(id))
            {
                Debug.LogError($"{nameof(id)} '{id}' is invalid. Expected the format 'skill.example_name'.", this);
                return false;
            }

            return true;
        }

        protected void ValidateRequiredReference(Object reference, string fieldName)
        {
            string assetPath = UnityEditor.AssetDatabase.GetAssetPath(this);
            string definitionLabel = string.IsNullOrEmpty(assetPath)
                ? $"{GetType().Name} '{name}'"
                : $"{GetType().Name} '{name}' ({assetPath})";

            if (reference == null)
            {
                Debug.LogError($"{definitionLabel}.{fieldName} is not assigned.", this);
            }
        }

        protected void ValidateFiniteNonNegative(float value, string fieldName)
        {
            if (value < 0f || float.IsNaN(value) || float.IsInfinity(value))
            {
                Debug.LogError($"{GetType().Name} '{name}'.{fieldName} must be finite and non-negative.", this);
            }
        }

        protected void ValidateFinitePositive(float value, string fieldName)
        {
            if (value <= 0f || float.IsNaN(value) || float.IsInfinity(value))
            {
                Debug.LogError($"{GetType().Name} '{name}'.{fieldName} must be finite and greater than zero.", this);
            }
        }

#endif
    }
}
