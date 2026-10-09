using System.Collections.Generic;
using Units.Skills;
using UnityEngine;

namespace Units.Buffs
{
    public readonly struct EnergyAbsorbBuffArgs : IBuffCreationArgs
    {
        public float EnergyAbsorbPerSkillCast { get; }
        public float Duration { get; }
        public BuffSource Source { get; }

        public EnergyAbsorbBuffArgs(float energyAbsorbPerSkillCast, float duration, BuffSource source)
        {
            EnergyAbsorbPerSkillCast = energyAbsorbPerSkillCast;
            Duration = duration;
            Source = source;
        }
    }

    public class EnergyAbsorb : Buff, IGlobalSkillCastTrigger
    {
        private float energyAbsorbPerSkillCast;
        public EnergyAbsorb(Model.Buffs.BuffDefinition definition, EnergyAbsorbBuffArgs args)
            : base(definition, args.Duration, args.Source.SourceUnit, args.Source.SourceSkill)
        {
            energyAbsorbPerSkillCast = args.EnergyAbsorbPerSkillCast;
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
                if (context.UnitManager == null)
                {
                    return;
                }

                // 向己方所有单位分配能量
                IReadOnlyList<Unit> allies = context.UnitManager.GetUnitsByFaction(context.faction);
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