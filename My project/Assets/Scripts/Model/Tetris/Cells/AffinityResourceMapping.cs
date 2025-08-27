using System;
using System.Collections.Generic;
using UnityEngine;

namespace Model.Tetri
{
    [CreateAssetMenu(fileName = "AffinityResourceMapping", menuName = "Tetris/AffinityResourceMapping")]
    public class AffinityResourceMapping : ScriptableObject
    {
        [Serializable]
        public struct AffinityResource
        {
            public AffinityType affinityType;
            public string name;
            [TextArea] public string description;
            public Sprite icon;
        }

        [SerializeField]
        private List<AffinityResource> affinityResources;

        private Dictionary<AffinityType, AffinityResource> affinityResourceDict;

        private void OnEnable()
        {
            InitializeDictionary();
        }

        private void InitializeDictionary()
        {
            affinityResourceDict = new Dictionary<AffinityType, AffinityResource>();
            if (affinityResources == null) return; // 防止空引用
            foreach (var res in affinityResources)
            {
                affinityResourceDict[res.affinityType] = res;
            }
        }

        public string GetName(AffinityType type)
        {
            return affinityResourceDict.TryGetValue(type, out var res) ? res.name : string.Empty;
        }

        public string GetDescription(AffinityType type)
        {
            return affinityResourceDict.TryGetValue(type, out var res) ? res.description : string.Empty;
        }

        public Sprite GetIcon(AffinityType type)
        {
            return affinityResourceDict.TryGetValue(type, out var res) ? res.icon : null;
        }

        public AffinityResource GetResource(AffinityType type)
        {
            return affinityResourceDict.TryGetValue(type, out var res) ? res : default;
        }
    }
}