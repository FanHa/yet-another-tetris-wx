using Model.Buffs;

namespace Units
{
    public interface IBuffVisual
    {
        void Initialize(Unit owner, BuffDefinition definition);
        void Activate();
        void Release();
    }
}