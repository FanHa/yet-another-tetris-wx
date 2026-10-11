using System.Collections.Generic;
using Units;

namespace Model.Battle.Statistics
{
    public sealed class DamageTypeSummary
    {
        public string DamageSource { get; }
        public float DamageValue { get; }
        public int DamageCount { get; }

        public DamageTypeSummary(string damageSource, float damageValue, int damageCount)
        {
            DamageSource = damageSource;
            DamageValue = damageValue;
            DamageCount = damageCount;
        }
    }

    public sealed class UnitDamageSummary
    {
        public string UnitName { get; }
        public Unit.Faction Faction { get; }
        public float TotalDamage { get; }
        public IReadOnlyList<DamageTypeSummary> DamageDetails { get; }

        public UnitDamageSummary(
            string unitName,
            Unit.Faction faction,
            float totalDamage,
            IReadOnlyList<DamageTypeSummary> damageDetails)
        {
            UnitName = unitName;
            Faction = faction;
            TotalDamage = totalDamage;
            DamageDetails = damageDetails;
        }
    }
}