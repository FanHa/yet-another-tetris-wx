using Model.Resonance;
using UnityEngine;

namespace Units
{
    public abstract class BuffVisual : MonoBehaviour, IBuffVisual
    {
        public abstract void Initialize(Unit owner, ResonanceType resonance);
        public abstract void Activate();
        public abstract void Release();
    }
}