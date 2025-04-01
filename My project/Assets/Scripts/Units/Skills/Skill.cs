using System;
using UnityEngine;

namespace Units.Skills
{
    public abstract class Skill
    {
        public string Name { get; private set; }
        protected float lastUsedTime = 0f;

        public bool IsReady()
        {
            return Time.time >= lastUsedTime + Cooldown();
        }

        public abstract void Execute(Unit caster);

        public abstract float Cooldown();

        public void Init(){
            // 初始化技能
            lastUsedTime = Time.time;
        }

    }
}