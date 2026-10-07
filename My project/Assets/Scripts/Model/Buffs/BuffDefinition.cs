using UnityEngine;
using Units;

namespace Model.Buffs
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Buffs/Definitions/Buff Definition")]
    public sealed class BuffDefinition : ScriptableObject
    {
        [SerializeField] private Component visualPrefab;

        public Component VisualPrefab => visualPrefab;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (visualPrefab != null && visualPrefab is not IBuffVisual)
            {
                Debug.LogError($"{nameof(visualPrefab)} must implement {nameof(IBuffVisual)}.", this);
            }
        }
#endif
    }
}