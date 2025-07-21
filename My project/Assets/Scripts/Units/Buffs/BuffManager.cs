using System;
using System.Collections.Generic;
using Model.Tetri;
using Unity.VisualScripting;
using UnityEngine;

namespace Units.Buffs
{
    public class BuffManager
    {
        public event Action<Buff> OnBuffApplied;
        public event Action<Buff> OnBuffRemoved;
        private readonly List<Buff> buffs = new List<Buff>();

        public void Tick(Unit unit, float interval)
        {
            for (int i = buffs.Count - 1; i >= 0; i--)
            {
                Buff buff = buffs[i];
                buff.UpdateTime(interval);
                if (buff is ITick tickBuff)
                {
                    tickBuff.OnTick(unit); // 调用ITick接口的OnTick方法
                }
                if (buff.IsExpired())
                {
                    OnBuffRemoved?.Invoke(buff);
                    buffs.RemoveAt(i);
                }
                    
            }
        }

        public void AddOrRefreshBuff(Buff buff)
        {
            var existing = buffs.Find(b => b.GetKey() == buff.GetKey());
            if (existing != null)
            {
                existing.RefreshDuration();
            }
            else
            {
                buffs.Add(buff);
                OnBuffApplied?.Invoke(buff);
            }
        }

        public void RemoveBuff(Buff buff)
        {
            if (buffs.Remove(buff))
            {
                OnBuffRemoved?.Invoke(buff);
            }
        }

        /// <summary>
        /// 获取所有Buff
        /// </summary>
        public IEnumerable<Buff> GetAllBuffs() => buffs;
    }
}