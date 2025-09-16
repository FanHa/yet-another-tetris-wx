using System;
using System.Collections.Generic;
using Model.Tetri;
using UnityEngine;
using UnityEngine.UI;

namespace UI.TetriInfo
{
    public class TetriInfo : MonoBehaviour
    {
        public event Action<Operation.Tetri> OnRotateTetriClicked;
        [SerializeField] private Operation.TetriFactory tetriFactory;
        [SerializeField] private GameObject normalTetriPanel;
        [SerializeField] private GameObject characterTetriPanel;
        [SerializeField] private Button closeButton;
        [SerializeField] private Button rotateButton;
        [SerializeField] private TetriCellTypeResourceMapping tetriCellTypeResourceMapping;
        [SerializeField] private AffinityResourceMapping affinityResourceMapping;
        [SerializeField] private ColorConfig affinityColorConfig;
        [SerializeField] private GameObject affinityObject;
        [SerializeField] private GameObject blocker;

        [Header("Skill")]
        [SerializeField] private Image skillIcon;
        [SerializeField] private TMPro.TextMeshProUGUI skillNameText;
        [SerializeField] private TMPro.TextMeshProUGUI skillDescriptionText;

        [Header("Affinity")]
        [SerializeField] private Image affinityIcon;
        [SerializeField] private TMPro.TextMeshProUGUI affinityDescriptionText;

        [Header("实例中引用")]
        [SerializeField] private Camera tetriCamera;

        private Operation.Tetri currentTetri;

        private void Awake()
        {
            closeButton.onClick.AddListener(HideTetriInfo);
            rotateButton.onClick.AddListener(HandleRotateButtonClicked);
        }

        private void Start()
        {
            normalTetriPanel.SetActive(false);
            characterTetriPanel.SetActive(false);
            closeButton.gameObject.SetActive(false);
            blocker.SetActive(false);
        }


        public void ShowTetriInfo(Model.Tetri.Tetri tetri)
        {
            blocker.SetActive(true);
            closeButton.gameObject.SetActive(true);
            if (currentTetri != null)
            {
                Destroy(currentTetri.gameObject);
                currentTetri = null;
            }
            bool isCharacter = tetri.Type == Model.Tetri.Tetri.TetriType.Character;
            normalTetriPanel.SetActive(!isCharacter);
            characterTetriPanel.SetActive(isCharacter);

            if (isCharacter)
                ShowCharacterTetri(tetri);
            else
                ShowNormalTetri(tetri);
        }

        private void ShowCharacterTetri(Model.Tetri.Tetri tetri)
        {
            // 角色面板占位，后续扩展
            // affinityObject.SetActive(false);
        }

        private void ShowNormalTetri(Model.Tetri.Tetri tetri)
        {
            // 原 normal 面板逻辑
            affinityObject.SetActive(true);

            currentTetri = tetriFactory.CreateTetri(tetri);
            currentTetri.transform.SetPositionAndRotation(tetriCamera.transform.position + tetriCamera.transform.forward * 5f, Quaternion.identity);

            Model.Tetri.Cell mainCell = tetri.GetMainCell();
            Sprite sprite = tetriCellTypeResourceMapping.GetSprite(mainCell);
            skillIcon.sprite = sprite;

            skillNameText.text = mainCell.Name();
            skillDescriptionText.text = mainCell.Description();

            Dictionary<AffinityType, int> affinityCounts = tetri.GetAffinityCounts();
            string affinityDesc = "";
            foreach (var kvp in affinityCounts)
            {
                var res = affinityResourceMapping.GetResource(kvp.Key);
                var colorEntry = affinityColorConfig.GetColorEntry(kvp.Key);

                if (affinityIcon != null)
                {
                    affinityIcon.color = colorEntry.maskColor;
                    var outline = affinityIcon.GetComponent<UnityEngine.UI.Outline>();
                    if (outline != null) outline.effectColor = colorEntry.borderColor;
                }
                affinityDesc += $"{res.name}: {res.description} ( X {kvp.Value} )\n";
                break; // 保持只展示一个代表项
            }
            affinityDescriptionText.text = affinityDesc.TrimEnd('\n');
        }

        public void HideTetriInfo()
        {

            if (currentTetri != null)
            {
                Destroy(currentTetri.gameObject);
                currentTetri = null;
            }
            normalTetriPanel.SetActive(false);
            characterTetriPanel.SetActive(false);
            blocker.SetActive(false);
            closeButton.gameObject.SetActive(false);
        }

        private void HandleRotateButtonClicked()
        {
            OnRotateTetriClicked?.Invoke(currentTetri);
        }
    }
}