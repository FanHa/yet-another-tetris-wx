using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Model.Resonance;
using UnityEngine.EventSystems;

namespace UI.UnitInfo
{
    public class Resonance : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Image icon;
        [SerializeField] private TMP_Text countText;
        [SerializeField] private ResonanceDatabase resonanceDatabase;

        public Action<ResonanceType, int> OnClicked;

        private ResonanceType currentType;
        private int currentCount;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (resonanceDatabase == null)
            {
                Debug.LogError($"{nameof(resonanceDatabase)} is not assigned.", this);
            }
        }
#endif

        public void SetResonance(ResonanceType type, int count)
        {
            currentType = type;
            currentCount = count;

            ResonanceDefinition entry = resonanceDatabase.GetDefinition(type);
            countText.text = "X" + count.ToString();
            icon.color = entry.BorderColor;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OnClicked?.Invoke(currentType, currentCount);
        }
    }
}