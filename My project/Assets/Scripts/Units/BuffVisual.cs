using Model.Buffs;
using UnityEngine;

namespace Units
{
    public abstract class BuffVisual : MonoBehaviour, IBuffVisual
    {
        public abstract void Initialize(Unit owner, BuffDefinition definition);
        public abstract void Activate();
        public abstract void Release();
    }
}