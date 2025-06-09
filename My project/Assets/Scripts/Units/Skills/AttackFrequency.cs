// using System.Collections;
// using System.Linq;
// using UnityEngine;

// namespace Units.Skills
// {
//     public class AttackFrequency : Skill
//     {
//         private Units.Buffs.AttackFrequency buffTemplate = new();

//         public override string Description()
//         {

//             // 返回技能描述
//             return $"为射程范围内的一个随机友方单位（包括自己）安装攻速模块," +
//                 $"{buffTemplate.Description()}";
//         }

//         protected override void ExecuteCore(Unit caster)
//         {
//             Unit targetAlly = FindRandomAlly(caster, caster.Attributes.AttackRange);
//             if (targetAlly == null)
//             {
//                 Debug.LogWarning("No valid allies found within range for AttackFrequency.");
//                 return;
//             }

//             targetAlly.AddBuff(new Units.Buffs.AttackFrequency());
//         }

//         public override string Name()
//         {
//             return "安装攻速模块"; // 返回技能名称
//         }
//     }
// }