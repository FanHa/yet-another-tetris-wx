using System.Collections.Generic;
using Model.Tetri;
using UnityEngine;
using UnityEngine.UI;

namespace UI.TetriInfo
{
    public class TetriInfo : MonoBehaviour
    {
        [SerializeField] private Operation.TetriFactory tetriFactory;
        [SerializeField] private GameObject panel;
        [SerializeField] private Button closeButton;
        [SerializeField] private TetriCellTypeResourceMapping tetriCellTypeResourceMapping;
        [SerializeField] private AffinityResourceMapping affinityResourceMapping;
        [SerializeField] private ColorConfig affinityColorConfig;
        [SerializeField] private GameObject affinityObject; 

        [Header("Skill")]
        [SerializeField] private Image skillIcon;
        [SerializeField] private TMPro.TextMeshProUGUI skillNameText;
        [SerializeField] private TMPro.TextMeshProUGUI skillDescriptionText;

        [Header("Affinity")]
        [SerializeField] private Image affinityIcon;
        [SerializeField] private TMPro.TextMeshProUGUI affinityDescriptionText;

        [Header("实例中引用")]
        [SerializeField] private Camera tetriCamera;

        private GameObject currentTetriObj;

        private void Awake()
        {
            closeButton.onClick.AddListener(HideTetriInfo);
        }

        private void Start()
        {
            panel.SetActive(false);
        }


        public void ShowTetriInfo(Model.Tetri.Tetri tetri)
        {
            panel.SetActive(true);

            if (currentTetriObj != null)
            {
                Destroy(currentTetriObj);
                currentTetriObj = null;
            }
            currentTetriObj = tetriFactory.CreateTetri(tetri).gameObject;
            currentTetriObj.transform.SetPositionAndRotation(tetriCamera.transform.position + tetriCamera.transform.forward * 5f, Quaternion.identity);

            Model.Tetri.Cell mainCell = tetri.GetMainCell();
            Sprite sprite = tetriCellTypeResourceMapping.GetSprite(mainCell);
            skillIcon.sprite = sprite;

            skillNameText.text = mainCell.Name();
            skillDescriptionText.text = mainCell.Description();

            if (tetri.Type == Model.Tetri.Tetri.TetriType.Character)
            {
                affinityObject.SetActive(false);
            }
            else
            {
                affinityObject.SetActive(true);
                Dictionary<AffinityType, int> affinityCounts = tetri.GetAffinityCounts();
                string affinityDesc = "";
                foreach (var kvp in affinityCounts)
                {
                    var res = affinityResourceMapping.GetResource(kvp.Key);
                    var colorEntry = affinityColorConfig.GetColorEntry(kvp.Key);
                    affinityIcon.color = colorEntry.maskColor;
                    var outline = affinityIcon.GetComponent<UnityEngine.UI.Outline>();
                    outline.effectColor = colorEntry.borderColor;

                    affinityDesc += $"{res.name}: {res.description} ( X {kvp.Value} )\n";
                    break;
                }
                affinityDescriptionText.text = affinityDesc.TrimEnd('\n');
            }

            
        }

        public void HideTetriInfo()
        {

            if (currentTetriObj != null)
            {
                Destroy(currentTetriObj);
                currentTetriObj = null;
            }
            panel.SetActive(false);
        }
    }
}