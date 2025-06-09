using UnityEngine;
namespace Units.Skills
{
    public class SkillEffectContext
    {
        public Skill Skill;
        public Unit Caster;
        public Unit Target;
        public Vector3 Position;
        public float Duration;
        public float Radius;
    }
}