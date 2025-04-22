using System;
using System.Collections.Generic;
using Units;
using UnityEngine;

namespace Model.Tetri
{
    [Serializable]
    public class GarbageReuse : Cell
    {
        public float AttackBonusPercentage = 5f;
        public float MaxHealthBonusPercentage = 5f;
        public float AttacksPerTenSecondsBonusPercentage = 5f;
        public float MoveSpeedBonusPercentage = 10f;
        public override void Apply(Unit unit)
        {
            // GarbageReuse 的 Apply 方法可以为空，逻辑在 Execute 中处理
            unit.Attributes.AttackPower.AddPercentageModifier(this, AttackBonusPercentage);
            unit.Attributes.MaxHealth.AddPercentageModifier(this, MaxHealthBonusPercentage);
            unit.Attributes.AttacksPerTenSeconds.AddPercentageModifier(this, AttacksPerTenSecondsBonusPercentage);
            unit.Attributes.MoveSpeed.AddPercentageModifier(this, MoveSpeedBonusPercentage);
        }

        /// <summary>
        /// 执行 GarbageReuse 的逻辑，处理 tetriCells 列表
        /// </summary>
        /// <param name="tetriCells">当前单元格列表</param>
        public void Reuse(List<Cell> tetriCells, Unit unit)
        {
            foreach (var cell in tetriCells)
            {
                if (cell is Padding)
                {
                    unit.Attributes.AttackPower.AddPercentageModifier(cell, AttackBonusPercentage);
                    unit.Attributes.MaxHealth.AddPercentageModifier(cell, MaxHealthBonusPercentage);
                    unit.Attributes.AttacksPerTenSeconds.AddPercentageModifier(cell, AttacksPerTenSecondsBonusPercentage);
                    unit.Attributes.MoveSpeed.AddPercentageModifier(cell, MoveSpeedBonusPercentage);
                }
            }
        }

        public override string Name()
        {
            return "垃圾回收";
        }

        public override string Description()
        {
            return $"每个Padding类型的格子提供：" +
                $"\n- 攻击力加成：{AttackBonusPercentage}%" +
                $"\n- 最大生命值加成：{MaxHealthBonusPercentage}%" +
                $"\n- 攻击速度加成：{AttacksPerTenSecondsBonusPercentage}%" +
                $"\n- 移动速度加成：{MoveSpeedBonusPercentage}%";
        
        }
    }
}