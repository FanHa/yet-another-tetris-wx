using UnityEngine;

namespace Units.Projectiles
{
    public class LifeBomb : MonoBehaviour
    {
        private Unit caster;
        private GameObject temporaryTarget;
        private float speed = 8f;
        private float healthAmount;
        private Skills.Skill sourceSkill;
        private bool isActive = false;
        [SerializeField]private float explosionRadius;
        private float explosionDelay = 1f; // 爆炸延迟秒数
        private float explosionTimer = 0f;
        private bool arrived = false;

        public void Init(Unit caster, GameObject temporaryTarget, float healthAmount, Skills.Skill sourceSkill)
        {
            this.caster = caster;
            this.temporaryTarget = temporaryTarget;
            this.healthAmount = healthAmount;
            this.sourceSkill = sourceSkill;
            isActive = true;
        }

        void Update()
        {
            if (!isActive)
                return;
            if (temporaryTarget == null)
            {
                Debug.LogWarning("[LifeBomb] Target is null! This should not happen.");
                return;
            }

            if (!arrived)
            {
                Vector3 direction = (temporaryTarget.transform.position - transform.position).normalized;
                transform.position += speed * Time.deltaTime * direction;
            }
            else
            {
                explosionTimer += Time.deltaTime;
                if (explosionTimer >= explosionDelay)
                {
                    HandleExplosion();
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject == temporaryTarget)
            {
                arrived = true;
                explosionTimer = 0f;
                speed = 0f;
                
            }
        }

        private void HandleExplosion()
        {
            var enemies = caster.UnitManager != null
                ? caster.UnitManager.FindEnemiesInRangeAtPosition(caster.faction, (Vector2)transform.position, explosionRadius)
                : null;

            if (enemies != null)
            {
                foreach (var unit in enemies)
                {
                    var damage = new Damages.Damage(healthAmount, Damages.DamageType.Skill);
                    damage.SetSourceUnit(caster);
                    damage.SetSourceLabel(sourceSkill != null ? sourceSkill.Name() : "生命炸弹");
                    damage.SetTargetUnit(unit);
                    unit.TakeDamage(damage);
                }
            }
            Destroy(temporaryTarget);
            Destroy(gameObject);
            

        }
    }
}
