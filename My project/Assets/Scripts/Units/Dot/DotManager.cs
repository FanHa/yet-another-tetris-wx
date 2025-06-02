using System;
using System.Collections.Generic;

namespace Units
{
    public class DotManager
    {
        public event Action<Damages.Damage> OnDotDamage; // (damage, type)
        public event Action<DotType> OnDotExplode;

        private readonly List<Dot> dots = new List<Dot>();

        // todo 堆叠爆炸机制再说
        private const int burnStackToExplode = 5; // 烈焰爆炸所需的堆叠数
        private const float burnExplodeDamage = 100f; // 烈焰爆炸伤害
        public DotManager()
        {

        }

        public void Tick(float interval)
        {
            for (int i = dots.Count - 1; i >= 0; i--)
            {
                Units.Dot dot = dots[i];
                if (dot.type == DotType.Burn)
                {
                    Damages.Damage damage = new Damages.Damage(dot.damagePerSecond * interval, Damages.DamageType.Dot);
                    damage.SetSourceLabel(dot.skill.Name()+ " - " + dot.label);
                    damage.SetSourceUnit(dot.caster);
                    OnDotDamage?.Invoke(damage);
                }
                dot.timeLeft -= interval;
                if (dot.timeLeft <= 0)
                    dots.RemoveAt(i);
            }

        }

        public void AddOrRefreshDot(Dot dot)
        {
            var existing = dots.Find(d => d.type == dot.type && d.skill == dot.skill && d.caster == dot.caster);
            if (existing != null)
            {
                existing.damagePerSecond = dot.damagePerSecond;
                existing.duration = dot.duration;
                existing.timeLeft = dot.duration;
            }
            else
            {
                dots.Add(dot);
            }
        }

        public int GetDotCount(DotType type) => dots.FindAll(d => d.type == type).Count;
    }
}