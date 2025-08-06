using System;
using Model.Tetri;
using UnityEngine;
using UnityEngine.UI;

namespace UI.UnitInfo
{
    public class Buff : MonoBehaviour
    {
        public event Action<Units.Buffs.Buff> OnBuffClicked;
        [SerializeField] private TetriCellTypeResourceMapping cellTypeResourceMapping;
        [SerializeField] private TMPro.TextMeshProUGUI remainTimeDurationText;
        [SerializeField] private Image buffIcon;
        [SerializeField] private Button buffButton;
        private Units.Buffs.Buff buff;

        private void Awake()
        {
            buffButton.onClick.AddListener(HandleClick);
        }

        private void Update()
        {
            if (buff == null)
                return;

            // -1为永久Buff，显示∞
            if (buff.TimeLeft < 0)
            {
                remainTimeDurationText.text = "∞";
            }
            else
            {
                remainTimeDurationText.text = Mathf.CeilToInt(buff.TimeLeft).ToString();
            }
        }
        /// <summary>
        /// 设置Buff图标
        /// </summary>
        public void SetBuff(Units.Buffs.Buff buff)
        {
            this.buff = buff;
            buffIcon.sprite = cellTypeResourceMapping.GetSprite(buff.SourceSkill.CellTypeId);
        }

        private void HandleClick()
        {
            OnBuffClicked?.Invoke(this.buff);
        }

    }
}