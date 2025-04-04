using System;
using Units;
namespace Model.Tetri
{
    [Serializable]
    public class PrecisionShoot : Cell, IBaseAttribute
    {
        public Units.Skills.PrecisionShoot skillInstance = new (); 
        public void ApplyAttributes(Unit unit)
        {
            unit.skills.Add(skillInstance);
        }

        public override string Description()
        {
            return "技能: 精准射击, 向射程内初始血量最低的敌人射出一支箭, 造成自身基础攻击*4的伤害,该技能冷却时间为10秒";
        }
    }
}