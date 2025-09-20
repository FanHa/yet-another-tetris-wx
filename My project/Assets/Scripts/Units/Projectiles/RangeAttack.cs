using UnityEngine;

namespace Units.Projectiles
{
    public class RangeAttack : MonoBehaviour
    {
        private Units.Unit caster;
        private Units.Unit target;

        public bool IsActive { get; private set; } = false;

        [SerializeField] private float speed;
        private SpriteRenderer spriteRenderer;
        void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        } 

        private void Update()
        {
            if (!IsActive)
                return;
            if (target == null || !target.IsActive)
            {
                Destroy(gameObject);
                return;
            }

            // 移动到目标
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);

            // 命中判定
            if (Vector3.Distance(transform.position, target.transform.position) < 0.1f)
            {
                HandleHitTarget();
            }
        }


        /// <summary>
        /// 初始化远程攻击投射物
        /// </summary>
        public void Init(Units.Unit caster, Units.Unit target)
        {

            this.caster = caster;
            spriteRenderer.sprite = caster.Fist1SpriteRenderer.sprite;
            this.target = target;
        }

        public void Activate()
        {
            IsActive = true;
        }

        private void HandleHitTarget()
        {
            float damageValue = caster.Attributes.AttackPower.finalValue;
            var damage = new Damages.Damage(damageValue, Damages.DamageType.Hit);
            damage.SetSourceUnit(caster);
            damage.SetTargetUnit(target);
            damage.SetSourceLabel(Name());

            caster.TriggerAttackHit(target, damage);
            target.TakeHit(caster, ref damage);

            Destroy(gameObject);
        }

        private string Name()
        {
            return "普通攻击";
        }
    }
}