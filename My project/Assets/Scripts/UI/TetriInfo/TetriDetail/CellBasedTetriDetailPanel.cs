
using System.Collections.Generic;
using Model.Resonance;
using UnityEngine;
using UnityEngine.UI;

namespace UI.TetriInfo
{
    public class CellBasedTetriDetailPanel : MonoBehaviour
    {
        [SerializeField] private Image skillIcon;
        [SerializeField] private TMPro.TextMeshProUGUI skillNameText;
        [SerializeField] private TMPro.TextMeshProUGUI skillDescriptionText;
        [SerializeField] private Image resonanceIcon;
        [SerializeField] private TMPro.TextMeshProUGUI resonanceDescriptionText;

        [SerializeField] private ResonanceDatabase resonanceDatabase;

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

            if (resonanceIcon == null)
            {
                Debug.LogError($"{nameof(resonanceIcon)} is not assigned.", this);
            }

            if (resonanceDescriptionText == null)
            {
                Debug.LogError($"{nameof(resonanceDescriptionText)} is not assigned.", this);
            }

            if (resonanceDatabase == null)
            {
                Debug.LogError($"{nameof(resonanceDatabase)} is not assigned.", this);
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

            Dictionary<ResonanceType, int> resonanceCounts = tetri.GetResonanceCounts();
            string resonanceDesc = "";
            foreach (var kvp in resonanceCounts)
            {
                var displayEntry = resonanceDatabase.GetDefinition(kvp.Key);
                resonanceIcon.color = displayEntry.MaskColor;
                var outline = resonanceIcon.GetComponent<UnityEngine.UI.Outline>();
                if (outline != null) outline.effectColor = displayEntry.BorderColor;
                resonanceDesc += $"{displayEntry.DisplayName}: {displayEntry.Description} ( X {kvp.Value} )\n";
                break;
            }
            resonanceDescriptionText.text = resonanceDesc.TrimEnd('\n');
        }

    }
}