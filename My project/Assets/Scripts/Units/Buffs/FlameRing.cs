using System.Linq;
using Model.Skills;
using Units.Skills;
using UnityEngine;

namespace Units.Buffs
{
    /// <summary>
    /// FlameRing Buff：每次Tick对周围一圈敌人施加Dot伤害
    /// </summary>
    public class FlameRing : Buff
    {
        private float dotDps;
        private float dotDuration;
        private float radius;

        public FlameRing(
            float dotDps,
            float dotDuration,
            float buffDuration,
            float radius,
            Unit sourceUnit,
            Skill sourceSkill
        ) : base(buffDuration, sourceUnit, sourceSkill)
        {
            this.dotDps = dotDps;
            this.dotDuration = dotDuration;
            this.radius = radius;
        }

        public override string Name() => "火环";
        public override string Description() =>
            $"对周围敌人施加灼烧DeBuff,每秒造成{dotDps}点伤害";


        public override void OnApply(IBuffContext context)
        {
            base.OnApply(context);
            var skillConfig = (FlameRingSkillConfig)sourceSkill.Definition.Config;
            var flameRingEntity = Object.Instantiate(
                skillConfig.ProjectilePrefab,
                context.SelfUnit.transform.position,
                Quaternion.identity,
                context.SelfUnit.transform
            );
            flameRingEntity.Initialize(
                owner: context.SelfUnit,
                radius: radius,
                sourceSkill: sourceSkill,
                dotDps: dotDps,
                dotDuration: dotDuration
            );
            flameRingEntity.Activate();
        }
    }
}