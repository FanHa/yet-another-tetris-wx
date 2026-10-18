namespace Units.States
{
    public interface IUnitState
    {
    }

    public interface IEnergyChargeableState : IUnitState
    {
        void AddEnergy(float amount);
    }
}