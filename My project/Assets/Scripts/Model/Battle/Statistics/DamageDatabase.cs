using System.Collections.Generic;
using System.Linq;
using Units.Damages;

namespace Model.Battle.Statistics
{
    public class DamageDatabase
    {
        private readonly List<Damage> damageRecords = new();

        public void AddDamage(Damage damage)
        {
            damageRecords.Add(damage);
        }

        public IReadOnlyList<UnitDamageSummary> GetDamageByUnit()
        {
            UnitDamageSummary[] summaries = damageRecords
                .GroupBy(damage => damage.SourceUnit)
                .Select(group =>
                {
                    DamageTypeSummary[] details = group
                        .GroupBy(damage => damage.SourceLabel)
                        .Select(subGroup => new DamageTypeSummary(
                            subGroup.Key,
                            subGroup.Sum(damage => damage.Value),
                            subGroup.Count()))
                        .OrderByDescending(detail => detail.DamageValue)
                        .ToList()
                        .ToArray();

                    return new UnitDamageSummary(
                        group.Key.name,
                        group.Key.faction,
                        group.Sum(damage => damage.Value),
                        System.Array.AsReadOnly(details));
                })
                .OrderByDescending(summary => summary.TotalDamage)
                .ToArray();

            return System.Array.AsReadOnly(summaries);
        }

        public void Reset()
        {
            damageRecords.Clear();
        }
    }
}