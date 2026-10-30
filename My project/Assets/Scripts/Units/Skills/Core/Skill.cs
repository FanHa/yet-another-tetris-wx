using Controller;
using Model.Skills;

namespace Units.Skills
{
    public abstract class Skill
    {

        public abstract string Name();
        public abstract string DescribeEffect();

        public SkillDefinition Definition { get; internal set; }
        public UnitManager UnitManager { get; internal set; }
        public Unit Owner { get; private set; }

        internal void Bind(Unit owner)
        {
            Owner = owner;
        }

        public virtual void OnActivate() { }
        public virtual void OnDeactivate() { }
        public virtual void OnUnitDeath() {}
        public virtual void OnPassiveEffectsApplication() { }
        public virtual void OnRelease() {}
    }

}