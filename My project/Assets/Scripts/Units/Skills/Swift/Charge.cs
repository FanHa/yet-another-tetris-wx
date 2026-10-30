using System.Linq;
using Model;
using Presentation;
using Model.Tetri;
using Model.Skills;
using UnityEngine;

namespace Units.Skills
{
    public class Charge : ActiveSkill
    {
        private readonly ChargeSkillDefinition skillDefinition;

        public Charge(ChargeSkillDefinition definition)
        {
            skillDefinition = definition;
            RequiredEnergy = definition.RequiredEnergy;
        }

        private struct ChargeStats
        {
            public StatBreakdownDisplay ChargeDamage;
        }


        private ChargeStats CalcStats()
        {
            float moveSpeed = Owner.Attributes.MoveSpeed.finalValue;
            AdditiveValueBreakdown chargeDamage = skillDefinition.CalculateDamage(moveSpeed);
            return new ChargeStats
            {
                ChargeDamage = new StatBreakdownDisplay(
                    StatLabel.DashDamage,
                    chargeDamage.BaseValue,
                    chargeDamage.AddedAmount
                ),
            };
        }

        protected override void ExecuteCore()
        {
            var stats = CalcStats();

            Vector2 ownerPosition = Owner.transform.position;
            Unit targetEnemy = UnitManager.GetActiveEnemies(Owner)
                .OrderByDescending(enemy => ((Vector2)enemy.transform.position - ownerPosition).sqrMagnitude)
                .FirstOrDefault();
            if (targetEnemy == null)
                return;

            var chargeProjectile = Object.Instantiate(
                skillDefinition.ProjectilePrefab,
                Owner.transform.position,
                Quaternion.identity
            );
            chargeProjectile.Init(
                owner: Owner,
                target: targetEnemy,
                speed: 2f * Owner.Attributes.MoveSpeed.finalValue,
                chargeDamage: stats.ChargeDamage.FinalValue,
                avoidancePriority: skillDefinition.AvoidancePriority,
                sourceSkill: this
            );
            chargeProjectile.Activate();
        }

        public override string DescribeEffect()
        {
            var stats = CalcStats();
            return
                Definition.Description +
                $"{stats.ChargeDamage}\n";
        }

        public override string Name() => Definition.DisplayName;
    }
}