// using System;
// using Units;
// namespace Model.Tetri
// {
//     [Serializable]
//     public class Freeze : Cell
//     {
//         public Units.Buffs.Freeze freezeInstance = new Units.Buffs.Freeze();
//         public override void Apply(Unit unit)
//         {
//             unit.attackEffects.Add(freezeInstance);
//         }

//         public override string Description()
//         {
//             return "攻击附带效果: " + freezeInstance.Description();
//         }

//         public override string Name()
//         {
//             return freezeInstance.Name();
//         }
//     }
// }