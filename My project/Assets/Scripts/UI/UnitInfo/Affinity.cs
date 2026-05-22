using System;
using System.Collections.Generic;
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
        [SerializeField] private Model.Tetri.ColorConfig colorConfig;

        public Action<AffinityType, int> OnClicked;

        private AffinityType currentType;
        private int currentCount;

        // 公共方法：传入亲和类型与数量，设置图标与文本
        public void SetAffinity(Model.Tetri.AffinityType type, int count)
        {
            currentType = type;
            currentCount = count;

            Model.Tetri.ColorConfig.AffinityColorEntry entry = colorConfig.GetColorEntry(type);
            countText.text = "X" + count.ToString();
            icon.color = entry.borderColor;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OnClicked?.Invoke(currentType, currentCount);
        }
    }
}