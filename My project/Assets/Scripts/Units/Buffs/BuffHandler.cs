using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Units.Buffs
{
    public class BuffHandler : MonoBehaviour
    {
        private float buffTickTimer = 0f;
        private const float BUFF_TICK_INTERVAL = 1f;
        private Unit owner;
        private readonly List<Buff> buffs = new List<Buff>();
        public event Action<Buff> BuffRemoved;

        private void Awake()
        {
            owner = GetComponent<Unit>();
        }

        private void Update()
        {
            if (buffs.Count == 0)
                return;
            buffTickTimer += Time.deltaTime;
            if (buffTickTimer >= BUFF_TICK_INTERVAL)
            {
                buffTickTimer -= BUFF_TICK_INTERVAL;
                Tick(BUFF_TICK_INTERVAL);
            }
        }

        private void Tick(float interval)
        {
            for (int i = buffs.Count - 1; i >= 0; i--)
            {
                Buff buff = buffs[i];
                buff.UpdateTime(interval);
                if (buff is ITick tickBuff)
                {
                    tickBuff.OnTick(owner);
                }
                if (buff.IsExpired())
                {
                    buff.OnRemove();
                    buffs.RemoveAt(i);
                    BuffRemoved?.Invoke(buff);
                }
            }
        }


        public void ApplyBuff(Buff buff)
        {
            var existing = buffs.Find(b => b.GetKey() == buff.GetKey());
            if (existing != null)
            {
                existing.RefreshDuration();
            }
            else
            {
                buff.OnApply(owner);
                buffs.Add(buff);
            }
        }

        public void RemoveBuff(Buff buff)
        {
            buffs.Remove(buff);
        }
        public IEnumerable<Buff> GetActiveBuffs()
        {
            return buffs;
        }
    }
}