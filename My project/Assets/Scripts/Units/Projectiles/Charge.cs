using System.Collections.Generic;
using UnityEngine;
using Units.Skills;

namespace Units.Projectiles
{
    public class Charge : MonoBehaviour
    {
        [SerializeField] private Controller.UnitManager unitManager;
        private Unit owner;
        private Unit target;
        private Vector3 targetPosition;
        private float speed;
        private float chargeDamage;
        private Skill sourceSkill;
        private HashSet<Unit> damagedUnits = new HashSet<Unit>();
        private bool isActive = false;
        private Unit.SelfSkillMotionLease skillMotionLease;
        [SerializeField] private float overshootDistance = 1f;
        [SerializeField] private float damageRadius = 1f;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (unitManager == null)
            {
                Debug.LogError($"{nameof(unitManager)} is not assigned.", this);
            }

            if (float.IsNaN(damageRadius) || float.IsInfinity(damageRadius) || damageRadius <= 0f)
            {
                Debug.LogError($"{nameof(damageRadius)} must be positive and finite.", this);
            }
        }
#endif

        public void Init(Unit owner, Unit target, float speed, float chargeDamage, Skill sourceSkill)
        {
            this.owner = owner;
            this.target = target;
            this.speed = speed;
            this.chargeDamage = chargeDamage;
            this.sourceSkill = sourceSkill;
            Vector3 dir = target.transform.position - owner.transform.position;
            if (dir.sqrMagnitude < 0.0001f)
                dir = target.transform.forward; // 同一点时用目标正向
            dir.Normalize();
            this.targetPosition = target.transform.position + dir * this.overshootDistance;
        }       

        public void Activate()
        {
            AcquireSkillMotion();
            isActive = true;
        }

        void Update()
        {
            if (!isActive)
                return;

            if (owner == null || !owner.IsActive || owner.IsStunned || !skillMotionLease.IsValid)
            {
                ReleaseSkillMotion();
                Destroy(gameObject);
                return;
            }

            Vector3 dir = (targetPosition - owner.transform.position).normalized;
            float step = speed * Time.deltaTime;
            var moveResult = owner.MoveBy(dir * step);

            if (moveResult.ShouldTerminate)
            {
                isActive = false;
                ReleaseSkillMotion();
                Destroy(gameObject);
                return;
            }

            var enemies = unitManager.FindEnemiesInRangeAtPosition(
                owner.faction,
                (Vector2)owner.transform.position,
                damageRadius
            );

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

            if (Vector3.Distance(owner.transform.position, targetPosition) < 0.1f)
            {
                isActive = false;
                ReleaseSkillMotion();
                Destroy(gameObject);
            }
        }

        private void AcquireSkillMotion()
        {
            if (skillMotionLease != null || owner == null)
            {
                return;
            }

            skillMotionLease = owner.AcquireSelfSkillMotion();
        }

        private void ReleaseSkillMotion()
        {
            if (skillMotionLease == null)
            {
                return;
            }

            skillMotionLease.Dispose();
            skillMotionLease = null;
        }

        private void OnDestroy()
        {
            ReleaseSkillMotion();
        }
    }
}