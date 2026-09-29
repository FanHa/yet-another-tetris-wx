using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Model.Tetri;
using UnityEngine.EventSystems;

namespace UI.UnitInfo
{
    public class Affinity : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Image icon;
        [SerializeField] private TMP_Text countText;
        [SerializeField] private Model.Tetri.AffinityDisplayConfig affinityDisplayConfig;

        public Action<AffinityType, int> OnClicked;

        private AffinityType currentType;
        private int currentCount;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (affinityDisplayConfig == null)
            {
                Debug.LogError($"{nameof(affinityDisplayConfig)} is not assigned.", this);
            }
        }
#endif

        // 公共方法：传入亲和类型与数量，设置图标与文本
        public void SetAffinity(Model.Tetri.AffinityType type, int count)
        {
            currentType = type;
            currentCount = count;

            Model.Tetri.AffinityDisplayConfig.AffinityDisplayEntry entry = affinityDisplayConfig.GetDisplayEntry(type);
            countText.text = "X" + count.ToString();
            icon.color = entry.BorderColor;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OnClicked?.Invoke(currentType, currentCount);
        }
    }
}