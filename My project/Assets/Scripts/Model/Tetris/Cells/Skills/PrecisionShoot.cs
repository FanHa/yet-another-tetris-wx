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
            unit.AddSkill(skillInstance);
        }

        public override string Description()
        {
            return $"向射程内初始血量最低的敌人射出一支箭," +
           $"造成基础攻击力 x {skillInstance.attackPowerMultiplier} 的伤害, " +
           $"冷却时间: {skillInstance.cooldown} 秒.";        
        }

        public override string Name()
        {
            return "精准打击";
        }
    }
}