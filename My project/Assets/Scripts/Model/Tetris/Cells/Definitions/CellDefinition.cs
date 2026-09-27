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
