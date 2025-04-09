using System;
using UnityEngine;

namespace Units.Skills
{
    public abstract class Skill
    {
        public string Name { get; private set; }
        protected float lastUsedTime = 0f;

        public virtual float cooldown { get; protected set; } = 0f;
        
        public bool IsReady()
        {
            return Time.time >= lastUsedTime + cooldown;
        }

        public abstract void Execute(Unit caster);

        public abstract string Description();

        public void Init(){
            // 初始化技能
            lastUsedTime = Time.time;
        }

        

    }
}