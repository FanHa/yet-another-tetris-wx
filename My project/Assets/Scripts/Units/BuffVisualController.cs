using System.Collections.Generic;
using Model.Skills;
using UnityEngine;
using Units.Buffs;

namespace Units
{
    [RequireComponent(typeof(Unit))]
    public sealed class BuffVisualController : MonoBehaviour
    {
        private readonly Dictionary<Buff, GameObject> visualsByBuff = new();
        private Unit unit;

        private void Awake()
        {
            unit = GetComponent<Unit>();
        }

        public void AddVisual(Buff buff)
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

        public void RemoveVisual(Buff buff)
        {
            if (!visualsByBuff.TryGetValue(buff, out var visual))
            {
                return;
            }

            visualsByBuff.Remove(buff);
            if (visual != null)
            {
                Destroy(visual);
            }
        }

        private GameObject CreateVisual(Buff buff)
        {
            Component prefab = buff switch
            {
                Freeze => ((IcyCageSkillConfig)buff.SourceSkill.Definition.Config).ProjectilePrefab,
                IceShield => ((IceShieldSkillConfig)buff.SourceSkill.Definition.Config).ProjectilePrefab,
                _ => null
            };

            if (prefab == null)
            {
                return null;
            }

            var visual = Instantiate(prefab, unit.transform.position, Quaternion.identity);
            var buffVisual = (IBuffVisual)visual;
            buffVisual.Initialize(unit);
            buffVisual.Activate();
            return visual.gameObject;
        }
    }
}