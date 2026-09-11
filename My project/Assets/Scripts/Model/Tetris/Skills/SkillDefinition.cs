using Units.Skills;
using UnityEngine;

namespace Model.Tetri
{
    [CreateAssetMenu(menuName = "Config/Skills/Skill Definition")]
    public sealed class SkillDefinition : ScriptableObject
    {
        [SerializeField] private string id;
        [SerializeField] private string displayName;
        [SerializeField] private string description;
        [SerializeField] private SkillConfig config;
        [SerializeField] private Sprite icon;

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
                id = name;
            }
        }
#endif
    }
}
