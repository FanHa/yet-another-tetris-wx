using System;
using System.Collections.Generic;
using System.Linq;
using Units.Damages;

namespace Controller
{
    public class DamageDatabase
    {
        private List<Damage> damageRecords = new();

        public void AddDamage(Damage damage)
        {
            damageRecords.Add(damage);
        }

        public List<(Units.Unit SourceUnit, float TotalDamage, List<(string DamageSource, float DamageValue, int DamageCount)> DamageDetails)> GetDamageByUnit()
        {
            return damageRecords
                .GroupBy(d => d.SourceUnit) // 按伤害来源单位分组
                .Select(group => new
                {
                    SourceUnit = group.Key,
                    TotalDamage = group.Sum(d => d.Value), // 计算总伤害
                    DamageDetails = group
                        .GroupBy(d => d.SourceLabel) // 按伤害来源名称分组
                        .Select(subGroup => new
                        {
                            DamageSource = subGroup.Key,
                            DamageValue = subGroup.Sum(d => d.Value), // 计算每种方式的总伤害
                            DamageCount = subGroup.Count() // 计算每种方式的伤害次数
                        })
                        .OrderByDescending(detail => detail.DamageValue) // 按伤害量排序
                        .ToList()
                })
                .OrderByDescending(unit => unit.TotalDamage) // 按总伤害排序
                .Select(unit => (
                    unit.SourceUnit,
                    unit.TotalDamage,
                    unit.DamageDetails.Select(detail => (detail.DamageSource, detail.DamageValue, detail.DamageCount)).ToList()
                ))
                .ToList();
        }

        public void Reset()
        {
            damageRecords.Clear();
        }
    }
}