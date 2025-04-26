using System.Collections.Generic;
using System.Linq;
using Units;
using UnityEngine;

namespace Controller
{
    public class UnitManager: MonoBehaviour
    {
        private List<Unit> factionA = new();
        private List<Unit> factionB = new();
        private List<Unit> deaded = new();

        [SerializeField] private GameObject unitPrefab;
        [SerializeField] private Transform factionAParent;
        [SerializeField] private Transform factionBParent;

        public void Register(Unit unit)
        {

            if (unit != null)
            {
                if (unit.faction == Unit.Faction.FactionA)
                {
                    factionA.Add(unit);
                }
                if (unit.faction == Unit.Faction.FactionB)
                {
                    factionB.Add(unit);
                }
                
            }
        }

        public void Unregister(Unit unit)
        {
            if (unit != null)
            {
                factionA.Remove(unit);
                factionB.Remove(unit);
            }
        }

        public void Reset()
        {
            factionA.Clear();
            factionB.Clear();
        }

        public List<Unit> GetFactionAUnits()
        {
            return factionA;
        }

        public List<Unit> GetFactionBUnits()
        {
            return factionB;
        }

        public List<Unit> GetAllUnits()
        {
            return factionA.Concat(factionB).ToList();
        }
    }
}