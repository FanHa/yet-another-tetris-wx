
using System.Collections.Generic;
using Model.Tetri;
using UnityEngine;
using UnityEngine.UI;

namespace UI.TetriInfo
{
    public class CellBasedTetriDetailPanel : MonoBehaviour
    {
        [SerializeField] private Image skillIcon;
        [SerializeField] private TMPro.TextMeshProUGUI skillNameText;
        [SerializeField] private TMPro.TextMeshProUGUI skillDescriptionText;
        [SerializeField] private Image affinityIcon;
        [SerializeField] private TMPro.TextMeshProUGUI affinityDescriptionText;

        [SerializeField] private AffinityDatabase affinityDisplayConfig;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (skillIcon == null)
            {
                Debug.LogError($"{nameof(skillIcon)} is not assigned.", this);
            }

            if (skillNameText == null)
            {
                Debug.LogError($"{nameof(skillNameText)} is not assigned.", this);
            }

            if (skillDescriptionText == null)
            {
                Debug.LogError($"{nameof(skillDescriptionText)} is not assigned.", this);
            }

            if (affinityIcon == null)
            {
                Debug.LogError($"{nameof(affinityIcon)} is not assigned.", this);
            }

            if (affinityDescriptionText == null)
            {
                Debug.LogError($"{nameof(affinityDescriptionText)} is not assigned.", this);
            }

            if (affinityDisplayConfig == null)
            {
                Debug.LogError($"{nameof(affinityDisplayConfig)} is not assigned.", this);
            }
        }
#endif

        public void BindData(View.Tetri.CellBasedTetriView pieceView)
        {
            var tetri = pieceView.Model;
            var mainCell = tetri.MainCell;

            skillIcon.sprite = mainCell.Icon;

            skillNameText.text = mainCell.Name();
            skillDescriptionText.text = mainCell.Description();

            Dictionary<AffinityType, int> affinityCounts = tetri.GetAffinityCounts();
            string affinityDesc = "";
            foreach (var kvp in affinityCounts)
            {
                var displayEntry = affinityDisplayConfig.GetEntry(kvp.Key);
                affinityIcon.color = displayEntry.MaskColor;
                var outline = affinityIcon.GetComponent<UnityEngine.UI.Outline>();
                if (outline != null) outline.effectColor = displayEntry.BorderColor;
                affinityDesc += $"{displayEntry.name}: {displayEntry.description} ( X {kvp.Value} )\n";
                break;
            }
            affinityDescriptionText.text = affinityDesc.TrimEnd('\n');
        }

    }
}