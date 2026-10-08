using System;
using System.Collections.Generic;
using Model.Buffs;
using UnityEngine;
using Units.Buffs;

namespace Units
{
    public sealed class BuffVisualController
    {
        private readonly Dictionary<Buff, GameObject> visualsByBuff = new();
        private readonly Unit unit;

        public BuffVisualController(Unit unit)
        {
            this.unit = unit ?? throw new ArgumentNullException(nameof(unit));
        }

        internal void Activate()
        {
            unit.OnBuffChanged += HandleBuffChanged;

            var activeBuffs = unit.GetActiveBuffsReadOnly();
            for (int index = 0; index < activeBuffs.Count; index++)
            {
                AddVisual(activeBuffs[index]);
            }
        }

        internal void Deactivate()
        {
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

            GameObject visual = CreateVisual(buff);
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
            if (visual != null)
            {
                UnityEngine.Object.Destroy(visual);
            }
        }

        private GameObject CreateVisual(Buff buff)
        {
            Component prefab = buff.Definition.VisualPrefab;
            if (prefab == null)
            {
                return null;
            }

            var visual = UnityEngine.Object.Instantiate(prefab, unit.transform.position, Quaternion.identity);
            var buffVisual = (IBuffVisual)visual;
            buffVisual.Initialize(unit);
            buffVisual.Activate();
            return visual.gameObject;
        }
    }
}