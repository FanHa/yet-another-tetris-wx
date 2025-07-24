using System.Linq;
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
            "对周围敌人施加灼烧DeBuff";


        public override void OnApply(Unit unit)
        {
            base.OnApply(unit);
            var prefab = unit.ProjectileConfig.FlameRingPrefab;
            var effectObj = Object.Instantiate(prefab, unit.transform.position, Quaternion.identity, unit.transform);
            var flameRingEntity = effectObj.GetComponent<Units.Projectiles.FlameRing>();
            flameRingEntity.Initialize(
                owner: unit,
                radius: radius,
                sourceSkill: sourceSkill,
                dotDps: dotDps,
                dotDuration: dotDuration
            );
            flameRingEntity.Activate();
        }
    }
}