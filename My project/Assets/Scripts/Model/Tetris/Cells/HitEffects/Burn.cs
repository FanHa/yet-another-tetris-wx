using System;
using Units;
namespace Model.Tetri
{
    [Serializable]
    public class Burn : Cell
    {
        public Units.Buffs.Burn burnInstance = new Units.Buffs.Burn(); // 实例化一个Burn对象
        public override void Apply(Unit unit)
        {   
            burnInstance.source = unit; // 设置Burn对象的source为当前单位
            unit.attackEffects.Add(burnInstance);
        }

        public override string Description()
        {
            return burnInstance.Description();
        }

        public override string Name()
        {
            return burnInstance.Name();
        }
    }
}