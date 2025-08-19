using UnityEngine;

namespace Units.Projectiles
{
    public abstract class ProjectileToUnit : MonoBehaviour
    {
        protected Units.Unit caster;
        protected Units.Unit target;
        [SerializeField] protected float speed;
        public bool IsActive { get; private set; } = false;

        public virtual void Init(Units.Unit caster, Units.Unit target)
        {
            this.caster = caster;
            this.target = target;
        }

        public void Activate()
        {
            IsActive = true;
        }

        protected virtual void Update()
        {
            if (!IsActive)
                return;
            if (target == null || !target.IsActive)
            {
                Destroy(gameObject);
                return;
            }
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
            if (Vector3.Distance(transform.position, target.transform.position) < 0.1f)
            {
                HandleHitTarget();
            }
        }

        protected abstract void HandleHitTarget();
    }
}