using Model.Resonance;

namespace Units
{
    public interface IBuffVisual
    {
        void Initialize(Unit owner, ResonanceType resonance);
        void Activate();
        void Release();
    }
}