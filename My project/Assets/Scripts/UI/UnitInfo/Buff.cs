using System;
using Model.Tetri;
using UnityEngine;
using UnityEngine.UI;

namespace UI.UnitInfo
{
    public class Buff : MonoBehaviour
    {
        public event Action<Units.Buffs.Buff> OnBuffClicked;
        private Image buffIcon;
        [SerializeField] private TetriCellTypeResourceMapping cellTypeResourceMapping;
        private Units.Buffs.Buff buff;

        private void Awake()
        {
            buffIcon = GetComponent<Image>();
            Button button = GetComponent<Button>();
            button.onClick.AddListener(HandleClick);
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