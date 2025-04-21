using System;
using Units;
using UnityEngine;

namespace Model.Tetri
{
    [Serializable]
    public class Circle : Character
    {
        [SerializeField] private float attackPowerValue = 7f; // 攻击力参数
        [SerializeField] private float maxCoreValue = 150f;

        public override string Name()
        {
            return "小圆";
        }
        public override string Description()
        {
            return $"攻击力: {attackPowerValue}, 生命值: {maxCoreValue}";
        }

        public override void Apply(Unit unit)
        {
            unit.Attributes.AttackPower.SetBaseValue(attackPowerValue);
            unit.Attributes.MaxHealth.SetBaseValue(maxCoreValue);
            unit.name = CharacterName;
            
        }


    }
}