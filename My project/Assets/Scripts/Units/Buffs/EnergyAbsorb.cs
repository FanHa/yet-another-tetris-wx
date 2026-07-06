using System.Collections.Generic;
using Units.Skills;
using UnityEngine;

namespace Units.Buffs
{
    public class EnergyAbsorb : Buff, IGlobalSkillCastTrigger
    {
        private float energyAbsorbPerSkillCast;
        public EnergyAbsorb(
            float energyAbsorbPerSkillCast,
            float duration,
            Unit sourceUnit,
            Skill sourceSkill
        ) : base(duration, sourceUnit, sourceSkill)
        {
            this.energyAbsorbPerSkillCast = energyAbsorbPerSkillCast;
        }
        // 护盾对象
        public override string Description()
        {
            return $"被动：每当敌对单位施放技能时，汲取{energyAbsorbPerSkillCast}点能量分配到己方所有单位";
        }

        public override string Name()
        {
            return "能量汲取";
        }

        public void OnGlobalSkillCast(IBuffContext context, Unit caster, Skill skill)
        {
            if (caster.faction != context.faction)
            {
                // 向己方所有单位分配能量
                List<Unit> allies = context.UnitManager.GetUnitsByFaction(context.faction);
                foreach (var ally in allies)
                {
                    if (ally.IsActive && ally != context.SelfUnit)
                    {
                        ally.AddSkillEnergy(energyAbsorbPerSkillCast);
                    }
                }
            }
        }
    }
}