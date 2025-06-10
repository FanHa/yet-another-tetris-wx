using System.Collections.Generic;
using UnityEngine;

namespace Model.Tetri
{
    [CreateAssetMenu(menuName = "Config/AffinityColorConfig")]
    public class ColorConfig : ScriptableObject
    {
        [System.Serializable]
        public class AffinityColorEntry
        {
            public AffinityType affinity;
            public Color borderColor = Color.white;
            public Color maskColor = new Color(1f, 1f, 1f, 0.3f);
        }

        [SerializeField]
        private AffinityColorEntry[] affinityColors;

        private Dictionary<AffinityType, AffinityColorEntry> colorDict;

        private void OnEnable()
        {
            // 构建字典，便于高效查找
            colorDict = new Dictionary<AffinityType, AffinityColorEntry>();
            foreach (var entry in affinityColors)
            {
                if (!colorDict.ContainsKey(entry.affinity))
                    colorDict.Add(entry.affinity, entry);
            }
        }

        public AffinityColorEntry GetColorEntry(AffinityType affinity)
        {
            if (colorDict == null || colorDict.Count != affinityColors.Length)
                OnEnable(); // 确保字典已初始化

            colorDict.TryGetValue(affinity, out var entry);
            return entry;
        }
    }
}