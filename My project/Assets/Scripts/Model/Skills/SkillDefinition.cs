using System.Text.RegularExpressions;
using Model.Resonance;
using Model.Tetri;
using UnityEngine;

namespace Model.Skills
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Skills/Definitions/Skill Definition")]
    public sealed class SkillDefinition : ScriptableObject
    {
        [SerializeField] private string id;
        [SerializeField] private string displayName;
        [SerializeField] private string description;
        [SerializeField] private AffinityType affinity;
        [SerializeField] private ResonanceType resonance = ResonanceType.None;
        [SerializeField] private SkillConfig config;
        [SerializeField] private Sprite icon;

#if UNITY_EDITOR
        private static readonly Regex IdPattern = new("^skill\\.[a-z][a-z0-9]*(?:_[a-z0-9]+)*$", RegexOptions.Compiled);
#endif
        public string Id => id;
        public string DisplayName => displayName;
        public string Description => description;
        public AffinityType Affinity => affinity;
        public ResonanceType Resonance => resonance;
        public SkillConfig Config => config;
        public Sprite Icon => icon;

    #if UNITY_EDITOR
        private void OnValidate()
        {
            if (!System.Enum.IsDefined(typeof(AffinityType), affinity) || affinity == AffinityType.End)
            {
                Debug.LogError($"{nameof(affinity)} must be a valid affinity other than '{AffinityType.End}'.", this);
            }

            if (!System.Enum.IsDefined(typeof(ResonanceType), resonance))
            {
                Debug.LogError($"{nameof(resonance)} '{resonance}' is invalid.", this);
            }

            if (string.IsNullOrWhiteSpace(id))
            {
                Debug.LogError($"{nameof(id)} is not assigned. Expected the format 'skill.example_name'.", this);
                return;
            }

            if (!IdPattern.IsMatch(id))
            {
                Debug.LogError($"{nameof(id)} '{id}' is invalid. Expected the format 'skill.example_name'.", this);
                return;
            }

            if (config == null)
            {
                Debug.LogError($"{nameof(config)} is not assigned.", this);
                return;
            }

        }

#endif
    }
}
