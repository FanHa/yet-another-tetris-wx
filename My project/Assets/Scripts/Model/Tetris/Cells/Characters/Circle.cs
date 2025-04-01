using System;
using Units;
using UnityEngine;

namespace Model.Tetri
{
    [Serializable]
    public class Circle : Character, IBaseAttribute
    {
        [SerializeField] private float attackPowerValue = 7f; // 攻击力参数
        [SerializeField] private float maxCoreValue = 180f;   
        public override string Description()
        {
            return $"攻击力: {attackPowerValue}, 生命值: {maxCoreValue}";
        }

        public override string CharacterDescription()
        {
            return Description();
        }

        public void ApplyAttributes(Unit unit)
        {
            unit.attackPower.SetBaseValue(attackPowerValue);
            unit.maxCore.SetBaseValue(maxCoreValue);
        }
    }
}