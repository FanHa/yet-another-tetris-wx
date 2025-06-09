using System.Linq;
using Units.Skills;
using UnityEngine;

namespace Units.Buffs
{
    /// <summary>
    /// FlameRing Buff：每次Tick对周围一圈敌人施加Dot伤害
    /// </summary>
    public class FlameRing : Buff, ITick
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

        public void OnTick(Unit self)
        {
            // 查找周围一圈敌人
            var enemies = self.enemyUnits
                .Where(u => u != null && Vector2.Distance(self.transform.position, u.transform.position) <= radius)
                .ToList();

            foreach (var enemy in enemies)
            {
                var dot = new Dot(
                    DotType.Burn,
                    skill: sourceSkill,
                    caster: self,
                    dps: dotDps,
                    duration: dotDuration,
                    label: "灼烧"
                );
                enemy.ApplyDot(dot);
            }
        }
    }
}