using System.Collections.Generic;
using UnityEngine;

namespace Model.Tetri
{
    [CreateAssetMenu(menuName = "Game Data/Shared/Affinity Display Config")]
    public class AffinityDisplayConfig : ScriptableObject
    {
        [System.Serializable]
        public class AffinityDisplayEntry
        {
            public AffinityType affinity;
            public string name;
            [TextArea] public string description;
            public Sprite icon;
            public Color baseColor = Color.white;

            public Color BorderColor => new(baseColor.r, baseColor.g, baseColor.b, 1f);
            public Color MaskColor => new(baseColor.r, baseColor.g, baseColor.b, MaskAlpha);
        }

        private const float MaskAlpha = 0.19607843f;

        [SerializeField] private AffinityDisplayEntry[] affinityEntries;

        private Dictionary<AffinityType, AffinityDisplayEntry> entriesByAffinity;

        private void OnEnable()
        {
            entriesByAffinity = new Dictionary<AffinityType, AffinityDisplayEntry>();
            foreach (var entry in affinityEntries)
            {
                if (!entriesByAffinity.ContainsKey(entry.affinity))
                    entriesByAffinity.Add(entry.affinity, entry);
            }
        }

        public AffinityDisplayEntry GetDisplayEntry(AffinityType affinity)
        {
            if (entriesByAffinity == null || entriesByAffinity.Count != affinityEntries.Length)
                OnEnable();

            entriesByAffinity.TryGetValue(affinity, out var entry);
            return entry;
        }

        public string GetName(AffinityType affinity)
        {
            return GetDisplayEntry(affinity)?.name ?? string.Empty;
        }

        public string GetDescription(AffinityType affinity)
        {
            return GetDisplayEntry(affinity)?.description ?? string.Empty;
        }

        public Sprite GetIcon(AffinityType affinity)
        {
            return GetDisplayEntry(affinity)?.icon;
        }
    }
}