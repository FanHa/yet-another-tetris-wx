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
        private int avoidancePriority;
        private Skill sourceSkill;
        private HashSet<Unit> damagedUnits = new HashSet<Unit>();
        private bool isActive = false;
        private bool hasAcquiredSkillMotion = false;
        [SerializeField] private float overshootDistance = 1f;


        public void Init(Unit owner, Unit target, float speed, float chargeDamage, int avoidancePriority, Skill sourceSkill)
        {
            this.owner = owner;
            this.target = target;
            this.speed = speed;
            this.chargeDamage = chargeDamage;
            this.avoidancePriority = avoidancePriority;
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

            if (owner == null || !owner.IsActive)
            {
                ReleaseSkillMotion();
                Destroy(gameObject);
                return;
            }

            Vector3 dir = (targetPosition - owner.transform.position).normalized;
            float step = speed * Time.deltaTime;
            var moveResult = owner.Movement.MoveStraightByDelta(dir * step);

            if (moveResult.ShouldTerminate)
            {
                isActive = false;
                ReleaseSkillMotion();
                Destroy(gameObject);
                return;
            }

            var enemies = owner.UnitManager.FindEnemiesInRangeAtPosition(
                owner.faction,
                (Vector2)owner.transform.position,
                1f // todo: 这里可以用合理的参数,而不是写死1f
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
                ReleaseSkillMotion();
                Destroy(gameObject);
            }
        }

        private void AcquireSkillMotion()
        {
            if (hasAcquiredSkillMotion || owner == null)
            {
                return;
            }

            owner.EnterSkillMotion(avoidancePriority);
            hasAcquiredSkillMotion = true;
        }

        private void ReleaseSkillMotion()
        {
            if (!hasAcquiredSkillMotion || owner == null)
            {
                return;
            }

            owner.ExitSkillMotion();
            hasAcquiredSkillMotion = false;
        }

        private void OnDestroy()
        {
            ReleaseSkillMotion();
        }
    }
}