using System.Collections.Generic;
using UnityEngine;
using Units.Skills;

namespace Units.Projectiles
{
    public class Charge : MonoBehaviour
    {
        private Unit owner;
        private Unit target;
        private Vector3 targetPosition;
        private float speed;
        private float chargeDamage;
        private Skill sourceSkill;
        private HashSet<Unit> damagedUnits = new HashSet<Unit>();
        private bool isActive = false;

        public void Init(Unit owner, Unit target, float speed, float chargeDamage, Skill sourceSkill)
        {
            this.owner = owner;
            this.target = target;
            this.speed = speed;
            this.chargeDamage = chargeDamage;
            this.sourceSkill = sourceSkill;
            this.targetPosition = target.transform.position;
        }       

        public void Activate()
        {
            isActive = true;
        }

        void Update()
        {
            if (!isActive)
                return;

            if (owner == null || !owner.IsActive)
            {
                Destroy(gameObject);
                return;
            }

            Vector3 dir = (targetPosition - owner.transform.position).normalized;
            float step = speed * Time.deltaTime;
            owner.transform.position += dir * step;

            var enemies = owner.UnitManager.FindEnemiesInRangeAtPosition(
                owner.faction,
                (Vector2)owner.transform.position,
                1f // 检测半径
            );

            if (enemies != null)
            {
                foreach (var unit in enemies)
                {
                    if (unit != null && unit != owner && !damagedUnits.Contains(unit))
                    {
                        var damage = new Damages.Damage(chargeDamage, Damages.DamageType.Skill);
                        damage.SetSourceUnit(owner);
                        damage.SetTargetUnit(unit);
                        damage.SetSourceLabel("冲锋");
                        unit.TakeDamage(damage);
                        damagedUnits.Add(unit);
                    }
                }
            }

            if (Vector3.Distance(owner.transform.position, targetPosition) < 0.1f)
            {
                isActive = false;
                Destroy(gameObject);
            }
        }
    }
}