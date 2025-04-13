using System.Collections;
using System.Linq;
using UnityEngine;

namespace Units.Skills
{
    public class FreezeShield : Skill
    {
        public override float cooldown => 10f; // 技能冷却时间
        private Units.Buffs.FreezeShield buffTemplate = new();

        public override string Description()
        {

            // 返回技能描述
            return $"为射程范围内的一个随机友方单位（包括自己）添加冰霜护盾," +
                $"护盾: {buffTemplate.Description()}" +
                $"技能冷却时间为 {cooldown} 秒";
        }

        protected override void ExecuteCore(Unit caster)
        {
            Unit targetAlly = FindRandomAlly(caster, caster.Attributes.AttackRange);
            if (targetAlly == null)
            {
                Debug.LogWarning("No valid allies found within range for FreezeShield.");
                return;
            }

            targetAlly.AddBuff(new Units.Buffs.FreezeShield());
        }

        public override string Name()
        {
            return "冰霜护盾"; // 返回技能名称
        }
    }
}