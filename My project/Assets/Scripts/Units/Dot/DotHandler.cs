using System.Collections.Generic;
using UnityEngine;

namespace Units
{
    /// <summary>
    /// 通用持续伤害（DoT）管理器，支持多种类型（如Burn、Poison等），以技能和施法者为区分
    /// </summary>
    public class DotHandler : MonoBehaviour
    {
        private DotManager dotManager;
        private float dotTickTimer = 0f;
        private const float DOT_TICK_INTERVAL = 1f;
        private Unit owner;

        private void Awake()
        {
            owner = GetComponent<Unit>();
            dotManager = new DotManager();
            dotManager.OnDotDamage += (damage) =>
            {
                damage.SetTargetUnit(owner); // 设置伤害目标为当前单位
                owner.TakeDamage(damage);
            };
        }

        private void Update()
        {
            if (dotManager.GetDotCount(DotType.Burn) == 0) return;
            dotTickTimer += Time.deltaTime;
            if (dotTickTimer >= DOT_TICK_INTERVAL)
            {
                dotTickTimer -= DOT_TICK_INTERVAL;
                dotManager.Tick(DOT_TICK_INTERVAL);
            }
        }

        public void ApplyDot(Dot dot)
        {
            dotManager.AddOrRefreshDot(dot);
        }
    }
}