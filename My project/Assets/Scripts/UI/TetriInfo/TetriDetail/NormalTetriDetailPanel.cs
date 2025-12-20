
using System.Collections.Generic;
using Model.Tetri;
using UnityEngine;
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
        [SerializeField] private Button rotateButton;
        
        [SerializeField] private TetriCellTypeResourceMapping tetriCellTypeResourceMapping;
        [SerializeField] private AffinityResourceMapping affinityResourceMapping;
        [SerializeField] private ColorConfig affinityColorConfig;

        public void BindData(Operation.Tetri tetriComponent)
        {
            Model.Tetri.Tetri tetri = tetriComponent.ModelTetri;
            Model.Tetri.Cell mainCell = tetri.GetMainCell();
            
            skillIcon.sprite = tetriCellTypeResourceMapping.GetSprite(mainCell);
            skillNameText.text = mainCell.Name();
            skillDescriptionText.text = mainCell.Description();

            Dictionary<AffinityType, int> affinityCounts = tetri.GetAffinityCounts();
            string affinityDesc = "";
            foreach (var kvp in affinityCounts)
            {
                var res = affinityResourceMapping.GetResource(kvp.Key);
                var colorEntry = affinityColorConfig.GetColorEntry(kvp.Key);
                affinityIcon.color = colorEntry.maskColor;
                var outline = affinityIcon.GetComponent<UnityEngine.UI.Outline>();
                if (outline != null) outline.effectColor = colorEntry.borderColor;
                affinityDesc += $"{res.name}: {res.description} ( X {kvp.Value} )\n";
                break;
            }
            affinityDescriptionText.text = affinityDesc.TrimEnd('\n');
        }

    }
}