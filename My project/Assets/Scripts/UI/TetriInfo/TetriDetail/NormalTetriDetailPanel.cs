
using System.Collections.Generic;
using Model.Tetri;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI.TetriInfo
{
    public class NormalTetriDetailPanel : MonoBehaviour, ITetriDetailPanel
    {
        [SerializeField] private Image skillIcon;
        [SerializeField] private TMPro.TextMeshProUGUI skillNameText;
        [SerializeField] private TMPro.TextMeshProUGUI skillDescriptionText;
        [SerializeField] private Image affinityIcon;
        [SerializeField] private TMPro.TextMeshProUGUI affinityDescriptionText;

        [SerializeField, FormerlySerializedAs("affinityColorConfig")]
        private AffinityDisplayConfig affinityDisplayConfig;

        public void BindData(Operation.Tetri tetriComponent)
        {
            Model.Tetri.Tetri tetri = tetriComponent.ModelTetri;
            Model.Tetri.Cell mainCell = tetri.GetMainCell();

            skillIcon.sprite = mainCell.Icon;

            skillNameText.text = mainCell.Name();
            skillDescriptionText.text = mainCell.Description();

            Dictionary<AffinityType, int> affinityCounts = tetri.GetAffinityCounts();
            string affinityDesc = "";
            foreach (var kvp in affinityCounts)
            {
                var displayEntry = affinityDisplayConfig.GetDisplayEntry(kvp.Key);
                affinityIcon.color = displayEntry.maskColor;
                var outline = affinityIcon.GetComponent<UnityEngine.UI.Outline>();
                if (outline != null) outline.effectColor = displayEntry.borderColor;
                affinityDesc += $"{displayEntry.name}: {displayEntry.description} ( X {kvp.Value} )\n";
                break;
            }
            affinityDescriptionText.text = affinityDesc.TrimEnd('\n');
        }

    }
}