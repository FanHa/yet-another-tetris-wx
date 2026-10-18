using System;
using System.Collections.Generic;
using System.Text;
using Model.Tetri;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI.TetriInfo
{
    public class TetriInfo : MonoBehaviour
    {
        [FormerlySerializedAs("normalTetriPanelPrefab")]
        [SerializeField] private GameObject cellBasedTetriPanelPrefab;
        [SerializeField] private GameObject characterTetriPanelPrefab;
        [SerializeField] private RectTransform detailSlotRoot;
        [SerializeField] private Button closeButton;

        [SerializeField] private GameObject blocker;

        [Header("实例中引用")]
        [SerializeField] private Camera tetriInfoCamera;
        [SerializeField] private Vector3 previewCameraOffset = new Vector3(0f, 0f, -10f);

        private View.Tetri.TetriView currentTetri;
        private CloseButtonPositionCoordinator closeButtonPositionCoordinator;

#if UNITY_EDITOR
        private void OnValidate()
        {
            float offsetLengthSquared = previewCameraOffset.sqrMagnitude;
            if (float.IsNaN(offsetLengthSquared) || float.IsInfinity(offsetLengthSquared))
            {
                Debug.LogError($"{nameof(previewCameraOffset)} must contain finite values with a finite squared length.", this);
            }

            if (cellBasedTetriPanelPrefab == null)
            {
                Debug.LogError($"{nameof(cellBasedTetriPanelPrefab)} is not assigned.", this);
            }
            else if (cellBasedTetriPanelPrefab.GetComponent<CellBasedTetriDetailPanel>() == null)
            {
                Debug.LogError($"{nameof(cellBasedTetriPanelPrefab)} must have a CellBasedTetriDetailPanel component.", this);
            }

            if (characterTetriPanelPrefab == null)
            {
                Debug.LogError($"{nameof(characterTetriPanelPrefab)} is not assigned.", this);
            }
            else if (characterTetriPanelPrefab.GetComponent<CharacterTetriDetailPanel>() == null)
            {
                Debug.LogError($"{nameof(characterTetriPanelPrefab)} must have a CharacterTetriDetailPanel component.", this);
            }
        }
#endif

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


        public void ShowTetriInfo(View.Tetri.TetriView tetriComponent)
        {
            currentTetri = tetriComponent;
            blocker.SetActive(true);
            closeButton.gameObject.SetActive(true);

            ClearDetailPanel();
            switch (tetriComponent)
            {
                case View.Tetri.CellBasedTetriView pieceView:
                    var cellBasedPanelInstance = Instantiate(cellBasedTetriPanelPrefab, detailSlotRoot);
                    LayoutRebuilder.ForceRebuildLayoutImmediate(detailSlotRoot);
                    cellBasedPanelInstance.GetComponent<CellBasedTetriDetailPanel>().BindData(pieceView);
                    break;
                case View.Tetri.TetriCharacterView characterView:
                    var characterPanelInstance = Instantiate(characterTetriPanelPrefab, detailSlotRoot);
                    LayoutRebuilder.ForceRebuildLayoutImmediate(detailSlotRoot);
                    characterPanelInstance.GetComponent<CharacterTetriDetailPanel>().BindData(characterView);
                    break;
                default:
                    throw new ArgumentException("Unsupported Tetri view type.", nameof(tetriComponent));
            }
            closeButtonPositionCoordinator.PositionCloseButtonAtTopRight();
            PositionPreviewCamera(tetriComponent.transform.position);
        }

        private void PositionPreviewCamera(Vector3 targetPosition)
        {
            tetriInfoCamera.transform.position = targetPosition + previewCameraOffset;
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