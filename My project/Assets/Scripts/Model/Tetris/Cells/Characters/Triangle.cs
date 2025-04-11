using System;
using Units;
using UnityEngine;

namespace Model.Tetri
{
    [Serializable]
    public class Triangle : Character
    {
        [SerializeField] private float attackPowerValue = 15f; // 攻击力参数
        [SerializeField] private float maxCoreValue = 60f;    
        public override string Description()
        {
            // 动态生成描述字符串，反映当前的攻击力和最大核心值
            return $"攻击力: {attackPowerValue}, 生命值: {maxCoreValue}";
        }

        public override void Apply(Unit unit)
        {
            unit.attackPower.SetBaseValue(attackPowerValue);
            unit.maxCore.SetBaseValue(maxCoreValue);
        }

        public override string Name()
        {
            return "三角形核心";
        }
    }
}