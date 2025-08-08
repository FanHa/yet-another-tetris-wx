using System.Collections.Generic;
using Model.Tetri.Skills;
using UnityEngine;

namespace Units.Projectiles
{
    public class WildWind : MonoBehaviour
    {
        private Unit caster;
        private float radius;
        private float duration;
        private float damage;
        private float debuffDuration;
        private int moveSlowPercent;
        private int atkReducePercent;
        private float timer = 0f;
        private bool initialized = false;
        private Units.Skills.Skill sourceSkill;

        private const float rotateSpeed = 60f; // 每秒旋转60度
        private const float pushSpeed = 2.5f;  // 每秒远离速度
        private HashSet<Unit> damagedUnits = new HashSet<Unit>();
        [SerializeField] private SpriteRenderer spriteRenderer;

        public void Initialize(
            Unit caster,
            float radius,
            float duration,
            float damage,
            float debuffDuration,
            int moveSlowPercent,
            int atkReducePercent,
            Units.Skills.Skill sourceSkill
        )
        {
            this.caster = caster;
            this.radius = radius;
            this.duration = duration;
            this.damage = damage;
            this.debuffDuration = debuffDuration;
            this.moveSlowPercent = moveSlowPercent;
            this.atkReducePercent = atkReducePercent;
            this.sourceSkill = sourceSkill;

            timer = 0f;
        }

        public void Activate()
        {
            initialized = true;
        }

        void Update()
        {
            if (!initialized)
                return;

            timer += Time.deltaTime;
            spriteRenderer.transform.Rotate(Vector3.forward, rotateSpeed * Time.deltaTime);

            var enemies = caster.UnitManager != null
                ? caster.UnitManager.FindEnemiesInRangeAtPosition(caster.faction, (Vector2)transform.position, radius)
                : null;
            if (enemies != null)
            {
                foreach (var unit in enemies)
                {
                    if (unit != null)
                    {
                        Vector3 dir = (unit.transform.position - transform.position).normalized;
                        // 计算当前与中心的向量
                        Vector3 toUnit = unit.transform.position - transform.position;
                        // 旋转向量
                        float angle = rotateSpeed * Time.deltaTime;
                        Vector3 rotated = Quaternion.Euler(0, 0, angle) * toUnit;
                        // 计算目标位置
                        Vector3 targetPos = transform.position + rotated + rotated.normalized * pushSpeed * Time.deltaTime;
                        // 移动unit
                        unit.transform.position = targetPos;
                        var buff = new Units.Buffs.WildWindDebuff(
                            duration: debuffDuration,
                            moveSlowPercent: moveSlowPercent,
                            atkReducePercent: atkReducePercent,
                            sourceUnit: caster,
                            sourceSkill: sourceSkill
                        );
                        unit.AddBuff(buff);

                        if (!damagedUnits.Contains(unit))
                        {
                            var dmg = new Damages.Damage(damage, Damages.DamageType.Skill);
                            dmg.SetSourceUnit(caster);
                            dmg.SetSourceLabel("狂风");
                            dmg.SetTargetUnit(unit);
                            unit.TakeDamage(dmg);
                            damagedUnits.Add(unit);
                        }

                    }
                }
            }

            

            if (timer >= duration)
            {
                Destroy(gameObject);
            }
        }
    }
}