using System;
using System.Collections.Generic;
using Model.Buffs;
using Model.Resonance;
using UnityEngine;
using Units.Buffs;

namespace Units
{
    public sealed class BuffVisualController
    {
        private readonly Dictionary<Buff, IBuffVisual> visualsByBuff = new();
        private readonly Unit unit;
        private bool isActive;

        public BuffVisualController(Unit unit)
        {
            this.unit = unit ?? throw new ArgumentNullException(nameof(unit));
        }

        internal void Activate()
        {
            if (isActive)
            {
                return;
            }

            isActive = true;
            unit.OnBuffChanged += HandleBuffChanged;

            var activeBuffs = unit.GetActiveBuffsReadOnly();
            for (int index = 0; index < activeBuffs.Count; index++)
            {
                AddVisual(activeBuffs[index]);
            }
        }

        internal void Deactivate()
        {
            isActive = false;
            unit.OnBuffChanged -= HandleBuffChanged;

            var trackedBuffs = new List<Buff>(visualsByBuff.Keys);
            for (int index = 0; index < trackedBuffs.Count; index++)
            {
                RemoveVisual(trackedBuffs[index]);
            }
        }

        private void HandleBuffChanged(UnitBuffChangedEvent buffChangedEvent)
        {
            if (buffChangedEvent.Owner != unit)
            {
                return;
            }

            if (buffChangedEvent.Kind == BuffChangeKind.Added)
            {
                AddVisual(buffChangedEvent.Buff);
            }
            else
            {
                RemoveVisual(buffChangedEvent.Buff);
            }
        }

        private void AddVisual(Buff buff)
        {
            if (visualsByBuff.ContainsKey(buff))
            {
                return;
            }

            IBuffVisual visual = CreateVisual(buff);
            if (visual != null)
            {
                visualsByBuff.Add(buff, visual);
            }
        }

        private void RemoveVisual(Buff buff)
        {
            if (!visualsByBuff.TryGetValue(buff, out var visual))
            {
                return;
            }

            visualsByBuff.Remove(buff);
            visual.Release();
        }

        private IBuffVisual CreateVisual(Buff buff)
        {
            BuffVisual prefab = buff.Definition.VisualPrefab;
            if (prefab == null)
            {
                return null;
            }

            BuffVisual buffVisual = UnityEngine.Object.Instantiate(prefab, unit.transform.position, Quaternion.identity);
            buffVisual.Initialize(unit, buff.SourceSkill.Definition.Resonance);
            buffVisual.Activate();
            return buffVisual;
        }
    }
}