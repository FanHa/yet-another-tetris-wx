// using System;

// namespace Units.Buffs
// {
//     public class AttackFrequency : Buff
//     {
//         private float attackSpeedIncreasePercentage = 50f; // 攻速增加百分比

//         public AttackFrequency()
//         {
//             durationSeconds = 10f; // 持续时间
//         }

//         public override string Name()
//         {
//             return "攻击频率模块";
//         }

//         public override string Description()
//         {
//             return $"增加目标攻击速度 {attackSpeedIncreasePercentage}%，持续 {durationSeconds} 秒";
//         }

//         public override void Apply(Unit unit)
//         {
//             unit.Attributes.AttacksPerTenSeconds.AddPercentageModifier(this, attackSpeedIncreasePercentage);
//         }

//         public override void Remove(Unit unit)
//         {
//             unit.Attributes.AttacksPerTenSeconds.RemovePercentageModifier(this);
//         }

//         public override void Affect(Unit unit)
//         {
//         }

//         public override Type TetriCellType => typeof(Model.Tetri.Skills.AttackFrequency); // Return the Type of the corresponding TetriCell
//     }
// }