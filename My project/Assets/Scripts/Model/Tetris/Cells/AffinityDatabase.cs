using System.Collections.Generic;
using UnityEngine;

namespace Model.Tetri
{
    [CreateAssetMenu(menuName = "Game Data/Shared/Affinity Database")]
    public class AffinityDatabase : ScriptableObject
    {
        [System.Serializable]
        public class AffinityEntry
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

        [SerializeField] private AffinityEntry[] affinityEntries;

        private Dictionary<AffinityType, AffinityEntry> entriesByAffinity;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (affinityEntries == null)
            {
                Debug.LogError($"{nameof(affinityEntries)} is not assigned.", this);
                return;
            }

            var seenAffinities = new HashSet<AffinityType>();
            for (int index = 0; index < affinityEntries.Length; index++)
            {
                var entry = affinityEntries[index];
                if (entry == null)
                {
                    Debug.LogError($"{nameof(affinityEntries)}[{index}] is not assigned.", this);
                    continue;
                }

                if (!System.Enum.IsDefined(typeof(AffinityType), entry.affinity) || entry.affinity == AffinityType.End)
                {
                    Debug.LogError($"{nameof(affinityEntries)}[{index}] has an invalid affinity '{entry.affinity}'.", this);
                    continue;
                }

                if (!seenAffinities.Add(entry.affinity))
                {
                    Debug.LogError($"Duplicate affinity entry for '{entry.affinity}'.", this);
                }

                if (string.IsNullOrWhiteSpace(entry.name))
                {
                    Debug.LogError($"Affinity name for '{entry.affinity}' is not assigned.", this);
                }

                if (string.IsNullOrWhiteSpace(entry.description))
                {
                    Debug.LogError($"Affinity description for '{entry.affinity}' is not assigned.", this);
                }
            }

            foreach (AffinityType affinity in System.Enum.GetValues(typeof(AffinityType)))
            {
                if (affinity != AffinityType.None && affinity != AffinityType.End && !seenAffinities.Contains(affinity))
                {
                    Debug.LogError($"Missing affinity entry for '{affinity}'.", this);
                }
            }
        }
#endif

        private void OnEnable()
        {
            entriesByAffinity = new Dictionary<AffinityType, AffinityEntry>();
            foreach (var entry in affinityEntries)
            {
                if (!entriesByAffinity.ContainsKey(entry.affinity))
                    entriesByAffinity.Add(entry.affinity, entry);
            }
        }

        public AffinityEntry GetEntry(AffinityType affinity)
        {
            if (entriesByAffinity == null || entriesByAffinity.Count != affinityEntries.Length)
                OnEnable();

            entriesByAffinity.TryGetValue(affinity, out var entry);
            return entry;
        }

        public string GetName(AffinityType affinity)
        {
            return GetEntry(affinity)?.name ?? string.Empty;
        }

        public string GetDescription(AffinityType affinity)
        {
            return GetEntry(affinity)?.description ?? string.Empty;
        }

        public Sprite GetIcon(AffinityType affinity)
        {
            return GetEntry(affinity)?.icon;
        }
    }
}