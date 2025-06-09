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
        public float damagePerSecond;
        public float duration;
        public float timeLeft;

        public string label;
        public Dot(DotType type, Skills.Skill skill, Unit caster, float dps, float duration, string label)
        {
            this.type = type;
            this.skill = skill;
            this.caster = caster;
            this.damagePerSecond = dps;
            this.duration = duration;
            this.timeLeft = duration;
            this.label = label;
        }
    }
}