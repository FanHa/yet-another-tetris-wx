namespace Units.Actions
{
    public enum AnimationEventType
    {
        AttackEnd,
        CastSkillEnd
    }

    public interface IAnimationEventHandler
    {
        void HandleAnimationEvent(AnimationEventType eventType);
    }
}
