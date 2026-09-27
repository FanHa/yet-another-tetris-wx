using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Model.Tetri
{
    [CreateAssetMenu(menuName = "Game Data/Shared/Affinities/Display Config")]
    public class AffinityDisplayConfig : ScriptableObject
    {
        [System.Serializable]
        public class AffinityDisplayEntry
        {
            public AffinityType affinity;
            public string name;
            [TextArea] public string description;
            public Sprite icon;
            public Color borderColor = Color.white;
            public Color maskColor = new Color(1f, 1f, 1f, 0.3f);
        }

        [SerializeField]
        [FormerlySerializedAs("affinityColors")]
        private AffinityDisplayEntry[] affinityEntries;

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