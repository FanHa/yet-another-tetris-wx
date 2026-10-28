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
            float moveSpeed = Context.Attributes.MoveSpeed.finalValue;
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

            Vector2 ownerPosition = Context.Transform.position;
            Unit targetEnemy = Context.GetActiveEnemies()
                .OrderByDescending(enemy => ((Vector2)enemy.transform.position - ownerPosition).sqrMagnitude)
                .FirstOrDefault();
            if (targetEnemy == null)
                return;

            var chargeProjectile = Object.Instantiate(
                skillDefinition.ProjectilePrefab,
                Context.Transform.position,
                Quaternion.identity
            );
            chargeProjectile.Init(
                owner: Context.Unit,
                target: targetEnemy,
                speed: 2f * Context.Attributes.MoveSpeed.finalValue,
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
                DescriptionStatic() +
                $"{stats.ChargeDamage}\n";
        }

        public override string Name() => NameStatic();

        public static string DescriptionStatic() => "冲向距离自己最远的敌人，沿途对敌人造成伤害";

        public static string NameStatic() => "冲锋";
    }
}