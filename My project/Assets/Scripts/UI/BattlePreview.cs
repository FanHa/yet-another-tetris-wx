using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    
    public class BattlePreview : MonoBehaviour
    {
        [SerializeField] private Button closeButton;
        public event Action OnClosed;
        private void Awake()
        {
            if (closeButton != null)
                closeButton.onClick.AddListener(HandleCloseButtonClicked);
        }

        private void OnDestroy()
        {
            if (closeButton != null)
                closeButton.onClick.RemoveListener(HandleCloseButtonClicked);
        }

        public void Activate() => SetActive(true);

        public void Deactivate() => SetActive(false);

        private void SetActive(bool value)
        {
            gameObject.SetActive(value);
        }

        private void HandleCloseButtonClicked()
        {
            OnClosed?.Invoke();
        }
    }
}