using System.Text.RegularExpressions;
using UnityEngine;
using Units;

namespace Model.Buffs
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Buffs/Definitions/Buff Definition")]
    public sealed class BuffDefinition : ScriptableObject
    {
        [SerializeField] private string id;
        [SerializeField] private string displayName;
        [TextArea, SerializeField] private string description;
        [SerializeField] private Sprite icon;
        [SerializeField] private Component visualPrefab;

        public string Id => id;
        public string DisplayName => displayName;
        public string Description => description;
        public Sprite Icon => icon;
        public Component VisualPrefab => visualPrefab;

#if UNITY_EDITOR
        private static readonly Regex IdPattern = new("^buff\\.[a-z][a-z0-9]*(?:_[a-z0-9]+)*$", RegexOptions.Compiled);

        private void OnValidate()
        {
            if (!string.IsNullOrWhiteSpace(id) && !IdPattern.IsMatch(id))
            {
                Debug.LogError($"{nameof(id)} '{id}' is invalid. Expected the format 'buff.example_name'.", this);
            }

            if (visualPrefab != null && visualPrefab is not IBuffVisual)
            {
                Debug.LogError($"{nameof(visualPrefab)} must implement {nameof(IBuffVisual)}.", this);
            }
        }
#endif
    }
}