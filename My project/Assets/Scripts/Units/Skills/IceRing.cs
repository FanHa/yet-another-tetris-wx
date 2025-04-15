using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Units.Skills
{
    public class IceRing : Skill
    {
        public override float cooldown => 15f; // 冰环技能冷却时间
        public float freezeDuration = 8f; // 冰冻持续时间
        public float effectRadius = 1f; // 冰环作用范围

        public override string Description()
        {
            return $"释放冰环，对周围 {effectRadius} 米范围内的敌人施加冰冻效果，" +
                   $"持续 {freezeDuration} 秒，技能冷却时间为 {cooldown} 秒。";
        }

        protected override void ExecuteCore(Unit caster)
        {
            List<Unit> enemies = FindEnemiesInRange(caster, effectRadius);
            foreach (Unit enemy in enemies)
            {
                if (enemy != null && enemy.faction != caster.faction)
                {
                    enemy.AddBuff(new Units.Buffs.DeepFreeze(freezeDuration));
                }
            }
        }

        public override string Name()
        {
            return "冰环";
        }
    }
}