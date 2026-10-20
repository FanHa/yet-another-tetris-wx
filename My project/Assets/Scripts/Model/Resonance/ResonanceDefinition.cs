using UnityEngine;

namespace Model.Resonance
{
    [CreateAssetMenu(menuName = "Game Data/Shared/Resonance Definition")]
    public sealed class ResonanceDefinition : ScriptableObject
    {
        private const float MaskAlpha = 0.19607843f;

        [SerializeField] private ResonanceType type;
        [SerializeField] private string displayName;
        [TextArea]
        [SerializeField] private string description;
        [SerializeField] private Sprite icon;
        [SerializeField] private Color baseColor = Color.white;

        public ResonanceType Type => type;
        public string DisplayName => displayName;
        public string Description => description;
        public Sprite Icon => icon;
        public Color BaseColor => baseColor;
        public Color BorderColor => new(baseColor.r, baseColor.g, baseColor.b, 1f);
        public Color MaskColor => new(baseColor.r, baseColor.g, baseColor.b, MaskAlpha);

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!System.Enum.IsDefined(typeof(ResonanceType), type) || type == ResonanceType.None || type == ResonanceType.End)
            {
                Debug.LogError($"{nameof(type)} must be a valid resonance other than '{ResonanceType.None}' or '{ResonanceType.End}'.", this);
            }

            if (string.IsNullOrWhiteSpace(displayName))
            {
                Debug.LogError($"{nameof(displayName)} is not assigned.", this);
            }

            if (string.IsNullOrWhiteSpace(description))
            {
                Debug.LogError($"{nameof(description)} is not assigned.", this);
            }
        }
#endif
    }
}