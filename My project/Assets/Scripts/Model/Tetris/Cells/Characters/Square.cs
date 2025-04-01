using System;
using Units;
using UnityEngine;

namespace Model.Tetri
{
    [Serializable]
    public class Square : Character, IBaseAttribute
    {
        [SerializeField] private float attackPowerValue = 10f; // 攻击力参数
        [SerializeField] private float maxCoreValue = 100f;   
        public override string Description()
        {
            return $"Attack Power: {attackPowerValue}, Max Core: {maxCoreValue}";
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