using System.Collections.Generic;
using Model.Tetri;
using UnityEngine;

namespace Units.Buffs
{
    public class Manager: MonoBehaviour
    {
        [SerializeField] private Transform buffViewerParent;
        [SerializeField] private BuffViewer buffViewerPrefab;
        [SerializeField] private TetriCellTypeResourceMapping tetriCellTypeResourceMapping;

        private Dictionary<string, Buff> activeBuffs = new Dictionary<string, Buff>();
        private Dictionary<string, BuffViewer> buffViewers = new Dictionary<string, BuffViewer>();

        public float DurationRevisePercentage;
        public void AddBuff(Buff buff, Unit unit)
        {
            if (activeBuffs.TryGetValue(buff.Name(), out var existingBuff))
            {
                // 如果状态已存在，则刷新持续时间
                existingBuff.RefreshDuration();
            }
            else
            {
                // 添加新的状态并立即应用
                activeBuffs[buff.Name()] = buff;
                buff.DurationRevisePercentage = DurationRevisePercentage;
                buff.RefreshDuration();
                buff.Apply(unit);

                // 创建 BuffViewer
                BuffViewer buffViewerInstance = Instantiate(buffViewerPrefab, buffViewerParent);
                buffViewerInstance.name = buff.Name();
                buffViewerInstance.SetBuffSprite(tetriCellTypeResourceMapping.GetSprite(buff.TetriCellType));
                buffViewers[buff.Name()] = buffViewerInstance;
            }
        }

        public void UpdateBuffs(Unit unit)
        {
            var expiredBuffs = new List<string>();
            foreach (var kvp in activeBuffs)
            {
                var buff = kvp.Value;
                if (buff.IsExpired())
                {
                    buff.Remove(unit);
                    expiredBuffs.Add(kvp.Key);

                    // 销毁 BuffViewer
                    if (buffViewers.TryGetValue(kvp.Key, out var buffViewer))
                    {
                        Destroy(buffViewer.gameObject);
                        buffViewers.Remove(kvp.Key);
                    }
                }
                else
                {
                    buff.Affect(unit);
                }
            }

            // 删除过期的 Buff
            foreach (var buffKey in expiredBuffs)
            {
                activeBuffs.Remove(buffKey);
            }
        }
    }
}