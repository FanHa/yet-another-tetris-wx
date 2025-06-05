using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Units.Skills
{
    public class Rush : Skill
    {
        public float rushDuration = 1f;
        public float speedMultiplier = 2f; // 冲刺速度倍数
        public float damageMultipierBySpeed = 10; // 伤害倍数
        private HashSet<Unit> hitEnemies = new HashSet<Unit>(); // 记录已碰撞的敌人


        protected override void ExecuteCore(Unit caster)
        {
            if (caster.enemyUnits == null || caster.enemyUnits.Count == 0)
            {
                Debug.LogWarning("No target enemies to rush towards.");
                return;
            }

            Transform targetEnemy = caster.enemyUnits[caster.enemyUnits.Count - 1].transform;
            // 开始冲刺协程
            caster.StartCoroutine(RushTowardsTarget(caster, targetEnemy));
        }

        private IEnumerator RushTowardsTarget(Unit caster, Transform targetEnemy)
        {
            Vector2 startPosition = caster.transform.position;
            Vector2 targetPosition = targetEnemy.position;

            float elapsedTime = 0f; // 已经过的时间
            float rushSpeed = caster.Attributes.MoveSpeed.finalValue * speedMultiplier; // 冲刺速度基于自身移动速度
            Vector2 direction = (targetPosition - startPosition).normalized; // 冲刺方向在循环外计算
            caster.moveable = false; // 暂时禁用其他行为

            while (elapsedTime < rushDuration)
            {
                // 计算冲刺的下一步位置
                Vector2 newPosition = (Vector2)caster.transform.position + direction * rushSpeed * Time.deltaTime;

                // 直接修改 Transform.position
                caster.transform.position = newPosition;

                // 检测与敌人的碰撞
                Collider2D[] hits = Physics2D.OverlapCircleAll(caster.transform.position, 0.3f);
                foreach (var hit in hits)
                {
                    Unit enemyUnit = hit.GetComponent<Unit>();
                    if (enemyUnit != null && enemyUnit.faction != caster.faction && !hitEnemies.Contains(enemyUnit))
                    {
                        Units.Damages.Damage damage = new Units.Damages.Damage(rushSpeed * damageMultipierBySpeed, Damages.DamageType.Skill);
                        damage.SetSourceLabel(Name());
                        damage.SetSourceUnit(caster);
                        damage.SetTargetUnit(enemyUnit);

                        // 对敌人造成伤害
                        enemyUnit.TakeDamage(damage);

                        // 记录已碰撞的敌人
                        hitEnemies.Add(enemyUnit);
                    }
                }

                // 更新已经过的时间
                elapsedTime += Time.deltaTime;

                yield return null; // 等待下一帧
            }

            // 冲刺结束，恢复单位行为
            caster.moveable = true;
            hitEnemies.Clear(); // 清空已碰撞的敌人记录
        }

        public override string Description()
        {
            return $"向最远的敌人冲锋，持续 {rushDuration} 秒，" +
                $"冲刺过程中与敌人碰撞会造成基于速度的伤害，" +
                $"伤害为速度的 {damageMultipierBySpeed} 倍。";
        }

        public override string Name()
        {
            return "冲锋";
        }
    }
}