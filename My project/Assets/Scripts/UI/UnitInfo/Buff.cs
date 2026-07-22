using System;
using Model.Tetri;
using UnityEngine;
using UnityEngine.UI;

namespace UI.UnitInfo
{
    public class Buff : MonoBehaviour
    {
        public event Action<Units.Buffs.Buff> OnBuffClicked;
        [SerializeField] private CellDatabase cellDatabase;
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
            if (cellDatabase != null && cellDatabase.TryGetSprite(buff.SourceSkill.CellTypeId.ToString(), out Sprite sprite))
            {
                buffIcon.sprite = sprite;
            }
        }

        private void HandleClick()
        {
            OnBuffClicked?.Invoke(this.buff);
        }

    }
}