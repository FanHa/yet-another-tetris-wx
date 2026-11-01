namespace Units.Skills
{
    public abstract class ActiveSkill : Skill
    {
        public float RequiredEnergy { get; protected set; }
        public float CurrentEnergy { get; private set; }
        public bool HasEnoughEnergy => CurrentEnergy >= RequiredEnergy;
        public float EnergyProgress => RequiredEnergy == 0f ? 1f : CurrentEnergy / RequiredEnergy;

        protected void InitializeInitialEnergy(float initialEnergy)
        {
            CurrentEnergy = initialEnergy;
        }

        internal void Execute()
        {
            ExecuteCore();
            CurrentEnergy = 0f;
        }

        internal void AddEnergy(float amount)
        {
            CurrentEnergy += amount;
            if (CurrentEnergy > RequiredEnergy)
            {
                CurrentEnergy = RequiredEnergy; // 确保不会超过最大能量
            }
        }

        internal void RemoveEnergy(float amount)
        {
            CurrentEnergy -= amount;
            if (CurrentEnergy < 0f)
            {
                CurrentEnergy = 0f;
            }
        }

        protected abstract void ExecuteCore();
    }
}
