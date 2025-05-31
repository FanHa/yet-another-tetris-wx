namespace Units
{
    public enum DotType
    {
        Burn,
        // Poison, Bleed, ... 可扩展
    }
    public class Dot
    {
        public DotType type;
        public Skills.Skill skill;
        public Unit caster;
        public int level;
        public float damagePerSecond;
        public float duration;
        public float timeLeft;

        public Dot(DotType type, Skills.Skill skill, Unit caster, int level, float dps, float duration)
        {
            this.type = type;
            this.skill = skill;
            this.caster = caster;
            this.level = level;
            this.damagePerSecond = dps;
            this.duration = duration;
            this.timeLeft = duration;
        }
    }
}