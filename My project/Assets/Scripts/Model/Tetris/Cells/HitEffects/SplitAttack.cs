using System;
using System.Collections.Generic;
using Units;
using Units.Damages;
using UnityEngine;

namespace Model.Tetri
{
    [Serializable]
    public class SplitAttack : Cell
    {
        [SerializeField]
        private float splitDamagePercentage = 30f; // 分裂攻击的伤害百分比
        [SerializeField]
        private float splitRange = 1f; // 分裂攻击的范围

        public override void Apply(Unit unit)
        {
            unit.OnAttackHit += HandleSplitAttack;
        }

        private void HandleSplitAttack(Damage damage)
        {
            if (damage.Type != DamageType.Hit)
                return;
            if (damage.TargetUnit == null || damage.SourceUnit == null) return;

            // 查找范围内的敌人
            Collider2D[] colliders = Physics2D.OverlapCircleAll(damage.TargetUnit.transform.position, splitRange);
            List<Unit> nearbyEnemies = new List<Unit>();

            foreach (var collider in colliders)
            {
                Unit enemy = collider.GetComponent<Unit>();
                if (enemy != null && enemy.faction != damage.SourceUnit.faction && enemy != damage.TargetUnit)
                {
                    nearbyEnemies.Add(enemy);
                }
            }

            // 对最多两个敌人发射投射物
            int splitCount = Mathf.Min(2, nearbyEnemies.Count);
            for (int i = 0; i < splitCount; i++)
            {
                Unit targetEnemy = nearbyEnemies[i];
                float splitDamageValue = damage.Value * (splitDamagePercentage / 100f);

                // 创建投射物
                GameObject projectileObject = UnityEngine.Object.Instantiate(
                    damage.SourceUnit.projectilePrefab,
                    damage.TargetUnit.transform.position,
                    Quaternion.identity
                );

                Units.Projectiles.Projectile projectile = projectileObject.GetComponent<Units.Projectiles.Projectile>();
                if (projectile != null)
                {
                    Damage splitDamage = new Damage(
                        splitDamageValue,
                        Name(),
                        DamageType.Skill,
                        damage.SourceUnit,
                        targetEnemy,
                        damage.SourceUnit.attackEffects
                    );
                    projectile.Init(damage.SourceUnit, targetEnemy.transform, 1.5f, splitDamage);
                    // todo 这一段代码与unit中创建projectile的代码有点冗余
                    SpriteRenderer projectileSpriteRenderer = projectileObject.GetComponent<SpriteRenderer>();
                    if (projectileSpriteRenderer != null && damage.SourceUnit.Fist1SpriteRenderer != null)
                    {
                        projectileSpriteRenderer.sprite = damage.SourceUnit.Fist1SpriteRenderer.sprite;
                    }
                    projectileObject.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
                }
            }
        }

        public override string Description()
        {
            return $"攻击击中目标后，从目标身上发射两个投射物，对附近敌人造成 {splitDamagePercentage}% 的伤害，范围为 {splitRange}。";
        }

        public override string Name()
        {
            return "分裂攻击";
        }
    }
}