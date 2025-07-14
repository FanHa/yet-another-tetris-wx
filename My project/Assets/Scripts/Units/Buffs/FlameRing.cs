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
            $"每{duration}s对半径{radius}内所有敌人施加{dotDps}/s灼烧({dotDuration}秒)";


        protected override void OnApplyExtra(Unit self)
        {
            var prefab = self.ProjectileConfig.FlameRingPrefab; // 你需要在配置里加上这个Prefab
            var effectObj = Object.Instantiate(prefab, self.transform.position, Quaternion.identity, self.transform);
            var flameRingEntity = effectObj.GetComponent<Units.Projectiles.FlameRing>();
            flameRingEntity.Initialize(
                owner: self,
                radius: radius,
                sourceSkill: sourceSkill,
                dotDps: dotDps,
                dotDuration: dotDuration
            );
            flameRingEntity.Activate();
        }
    }
}