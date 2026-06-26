namespace Units.Actions
{
    public readonly struct ActionTickContext
    {
        public float DeltaTime { get; }

        public float TimelineSpeed { get; }

        public ActionTickContext(float deltaTime, float timelineSpeed)
        {
            DeltaTime = deltaTime;
            TimelineSpeed = timelineSpeed;
        }
    }
}