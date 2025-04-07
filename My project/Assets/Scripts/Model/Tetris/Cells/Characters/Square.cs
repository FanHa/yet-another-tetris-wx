using System;
using Units;
using UnityEngine;

namespace Model.Tetri
{
    [Serializable]
    public class Square : Character
    {
        [SerializeField] private float attackPowerValue = 10f; // 攻击力参数
        [SerializeField] private float maxCoreValue = 100f;   
        public override string Description()
        {
            return $"攻击力: {attackPowerValue}, 生命值: {maxCoreValue}";
        }

        public override void ApplyCharacterFeature(Unit unit)
        {
            unit.attackPower.SetBaseValue(attackPowerValue);
            unit.maxCore.SetBaseValue(maxCoreValue);
        }

        public override string Name()
        {
            return "方形核心";
        }
    }
}