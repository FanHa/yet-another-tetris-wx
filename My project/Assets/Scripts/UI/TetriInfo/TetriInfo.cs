using System;
using System.Collections.Generic;
using System.Text;
using Model.Tetri;
using UnityEngine;
using UnityEngine.UI;

namespace UI.TetriInfo
{
    public class TetriInfo : MonoBehaviour
    {
        [SerializeField] private GameObject normalTetriPanelPrefab;
        [SerializeField] private GameObject characterTetriPanelPrefab;
        [SerializeField] private RectTransform detailSlotRoot;
        [SerializeField] private Button closeButton;

        [SerializeField] private GameObject blocker;

        [Header("实例中引用")]
        [SerializeField] private Camera tetriInfoCamera;

        private Operation.Tetri currentTetri;
        private CloseButtonPositionCoordinator closeButtonPositionCoordinator;

        private void Awake()
        {
            closeButton.onClick.AddListener(HideTetriInfo);
            closeButtonPositionCoordinator = GetComponent<CloseButtonPositionCoordinator>();
        }

        private void Start()
        {
            ClearDetailPanel();
            closeButton.gameObject.SetActive(false);
            blocker.SetActive(false);
        }


        public void ShowTetriInfo(Operation.Tetri tetriComponent)
        {
            currentTetri = tetriComponent;
            blocker.SetActive(true);
            closeButton.gameObject.SetActive(true);

            ClearDetailPanel();
            bool isCharacter = tetriComponent.ModelTetri.Type == Model.Tetri.Tetri.TetriType.Character;
            var prefab = isCharacter ? characterTetriPanelPrefab : normalTetriPanelPrefab;
            var detailPanelInstance =Instantiate(prefab, detailSlotRoot);
            LayoutRebuilder.ForceRebuildLayoutImmediate(detailSlotRoot);
            var detailPanel = detailPanelInstance.GetComponent<ITetriDetailPanel>();
            detailPanel.BindData(tetriComponent);
            closeButtonPositionCoordinator.PositionCloseButtonAtTopRight();
                
            Vector3 offset = new Vector3(0f, 0f, -10f);
            tetriInfoCamera.transform.position = tetriComponent.transform.position + offset;

        }

        private void ClearDetailPanel()
        {
            foreach (Transform child in detailSlotRoot)
            {
                Destroy(child.gameObject);
            }
        }


        public void HideTetriInfo()
        {
            ClearDetailPanel();
            blocker.SetActive(false);
            closeButton.gameObject.SetActive(false);
        }
    }
}