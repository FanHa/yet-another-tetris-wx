using System;
using Units;
namespace Model.Tetri
{
    [Serializable]
    public class Bomb : Cell, IBaseAttribute
    {
        public Units.Skills.Bomb skillInstance = new (); // 实例化一个Burn对象
        public void ApplyAttributes(Unit unit)
        {
            unit.skills.Add(skillInstance);
        }

        public override string Description()
        {
            return "技能: 爆破炸弹," +
                   "技能冷却时间: " + skillInstance.Cooldown() + "秒," +
                   "技能伤害: " + skillInstance.damage + "," +
                   "技能范围: " + skillInstance.explosionRadius + ",";
        }
    }
}