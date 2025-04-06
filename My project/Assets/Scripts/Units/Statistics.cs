
using System.Collections.Generic;

namespace Units
{
    public class Statistics
    {
        public string UnitName { get; } // 使用单位的唯一标识
        public Dictionary<string, float> DamageByType { get; } = new Dictionary<string, float>();

        public Statistics(string unitName)
        {
            UnitName = unitName;
        }

        public void AddDamage(string damageType, float damageValue)
        {
            if (!DamageByType.ContainsKey(damageType))
            {
                DamageByType[damageType] = 0f;
            }
            DamageByType[damageType] += damageValue;
        }
    }
}