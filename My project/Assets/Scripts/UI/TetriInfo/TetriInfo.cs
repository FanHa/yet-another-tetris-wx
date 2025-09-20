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

        [Header("Character")]
        [SerializeField] private TMPro.TextMeshProUGUI characterNameText;
        [SerializeField] private TMPro.TextMeshProUGUI characterDescriptionText;
        [SerializeField] private TMPro.TextMeshProUGUI characterAttributeText;

        [Header("实例中引用")]
        [SerializeField] private Camera tetriInfoCamera;

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


        public void ShowTetriInfo(Operation.Tetri tetriComponent)
        {
            currentTetri = tetriComponent;
            blocker.SetActive(true);
            closeButton.gameObject.SetActive(true);

            bool isCharacter = tetriComponent.ModelTetri.Type == Model.Tetri.Tetri.TetriType.Character;
            normalTetriPanel.SetActive(!isCharacter);
            characterTetriPanel.SetActive(isCharacter);
            Vector3 offset = new Vector3(0f, 0f, -10f);
            tetriInfoCamera.transform.position = tetriComponent.transform.position + offset;
            if (isCharacter)
                ShowCharacterTetri(tetriComponent);
            else
                ShowNormalTetri(tetriComponent);
        }

        private void ShowCharacterTetri(Operation.Tetri tetriComponent)
        {
            Model.Tetri.Tetri tetri = tetriComponent.ModelTetri;
            Model.Tetri.Character mainCell = tetri.GetMainCell() as Model.Tetri.Character;
            characterNameText.text = mainCell.CharacterName;
            characterDescriptionText.text = mainCell.Description();
            var character = tetriComponent as Operation.TetriCharacter;
            characterAttributeText.text = BuildAttributesText(character.PreviewUnit.Attributes);

            
        }

        private string BuildAttributesText(Units.Attributes a)
        {
            // 按行拼接：生命/护盾/攻速/攻距/移速/能量等
            var sb = new StringBuilder(128);

            // 生命
            sb.AppendLine($"生命: {a.CurrentHealth:0}/{a.MaxHealth.finalValue:0}");

            // 攻击力与攻速（攻速：每10秒与每秒）
            sb.AppendLine($"攻击力: {a.AttackPower.finalValue:0.##}");
            sb.AppendLine($"攻速: {a.AttacksPerTenSeconds.finalValue:0.##}/10s ({a.AttacksPerTenSeconds.finalValue / 10f:0.##}/s)");

            // 攻击范围
            sb.AppendLine($"攻击范围: {a.AttackRange.finalValue:0.##}");

            // 移速与能量回复
            sb.AppendLine($"移速: {a.MoveSpeed.finalValue:0.##}");
            sb.AppendLine($"能量回复: {a.EnergyPerSecond.finalValue:0.##}/s");

            return sb.ToString();
        }

        private void ShowNormalTetri(Operation.Tetri tetriComponent)
        {
            // 原 normal 面板逻辑
            affinityObject.SetActive(true);
            Model.Tetri.Tetri tetri = tetriComponent.ModelTetri;

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