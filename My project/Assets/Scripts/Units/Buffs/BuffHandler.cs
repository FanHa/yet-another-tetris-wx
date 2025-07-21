using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Units.Buffs
{
    public class BuffHandler : MonoBehaviour
    {
        private BuffManager buffManager;
        private float buffTickTimer = 0f;
        private const float BUFF_TICK_INTERVAL = 1f;
        private Unit owner;

        private void Awake()
        {
            owner = GetComponent<Unit>();
            buffManager = new BuffManager();
            buffManager.OnBuffApplied += (buff) =>
            {
                buff.OnApply(owner); // 应用Buff效果
            };
            buffManager.OnBuffRemoved += (buff) =>
            {
                buff.OnRemove(owner); // 移除Buff效果
            };
        }

        private void Update()
        {
            if (buffManager.GetAllBuffs().Count() == 0)
                return;
            buffTickTimer += Time.deltaTime;
            if (buffTickTimer >= BUFF_TICK_INTERVAL)
            {
                buffTickTimer -= BUFF_TICK_INTERVAL;
                buffManager.Tick(owner, BUFF_TICK_INTERVAL);
            }
        }

        public void ApplyBuff(Buff buff)
        {
            buffManager.AddOrRefreshBuff(buff);
        }

        public void RemoveBuff(Buff buff)
        {
            buffManager.RemoveBuff(buff);
        }

        public IEnumerable<Buff> GetActiveBuffs()
        {
            return buffManager.GetAllBuffs();
        }
    }
}