using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace UI
{


    public class TabsManager : MonoBehaviour
    {
        [SerializeField] private Button usableTab;
        [SerializeField] private Button usedTab;
        [SerializeField] private Button unusedTab;

        [SerializeField] private GameObject usablePanel;
        [SerializeField] private GameObject usedPanel;
        [SerializeField] private GameObject unusedPanel;

        void Start()
        {
            // Show default tab
            SwitchTab(0);
            EventSystem.current.SetSelectedGameObject(usableTab.gameObject);
        }

        public void SwitchTab(int tabIndex)
        {
            // Hide all panels
            usablePanel.SetActive(false);
            usedPanel.SetActive(false);
            unusedPanel.SetActive(false);

            // Show selected panel
            switch (tabIndex)
            {
                case 0:
                    usablePanel.SetActive(true);
                    break;
                case 1:
                    usedPanel.SetActive(true);
                    break;
                case 2:
                    unusedPanel.SetActive(true);
                    break;
            }

        }
    }
}