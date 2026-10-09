using System.Text.RegularExpressions;
using UnityEngine;

namespace Model.Skills
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Skills/Definitions/Skill Definition")]
    public sealed class SkillDefinition : ScriptableObject
    {
        [SerializeField] private string id;
        [SerializeField] private string displayName;
        [SerializeField] private string description;
        [SerializeField] private SkillConfig config;
        [SerializeField] private Sprite icon;

#if UNITY_EDITOR
        private static readonly Regex IdPattern = new("^skill\\.[a-z][a-z0-9]*(?:_[a-z0-9]+)*$", RegexOptions.Compiled);
#endif
        public string Id => id;
        public string DisplayName => displayName;
        public string Description => description;
        public SkillConfig Config => config;
        public Sprite Icon => icon;

    #if UNITY_EDITOR
        private void OnValidate()
        {
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
