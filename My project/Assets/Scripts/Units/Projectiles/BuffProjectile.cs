using Model.Tetri;
using UnityEngine;

namespace Units.Projectiles
{
    public class BuffProjectile : MonoBehaviour
    {
        private Unit caster;
        private Unit target;
        private Units.Buffs.Buff buff;
        [SerializeField] private float moveSpeed;
        [SerializeField] private float growDuration;
        [SerializeField] private float preMoveScale;
        [SerializeField] private float shrinkDuration;
        private float timer = 0f;
        private bool isActive = false;

        private enum BuffProjectilePhase
        {
            Growing,
            Moving,
            Shrinking
        }
        private BuffProjectilePhase phase = BuffProjectilePhase.Growing;
        private SpriteRenderer spriteRenderer;
        [SerializeField] private TetriCellTypeResourceMapping cellTypeResourceMapping;

        void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        

        void Update()
        {
            if (!isActive)
                return;
            switch (phase)
            {
                case BuffProjectilePhase.Growing:
                    timer += Time.deltaTime;
                    float tGrow = Mathf.Clamp01(timer / growDuration);
                    transform.localScale = Vector3.one * Mathf.Lerp(0.3f, preMoveScale, tGrow);
                    SetAlpha(Mathf.Lerp(0f, 1f, tGrow));
                    if (tGrow >= 1f)
                    {
                        phase = BuffProjectilePhase.Moving;
                        timer = 0f;
                    }
                    break;
                case BuffProjectilePhase.Moving:
                    transform.localScale = Vector3.one * preMoveScale;
                    SetAlpha(1f);
                    if (target != null)
                    {
                        Vector3 dir = (target.transform.position - transform.position).normalized;
                        transform.position += moveSpeed * Time.deltaTime * dir;
                        if (Vector3.Distance(transform.position, target.transform.position) < 0.2f)
                        {
                            phase = BuffProjectilePhase.Shrinking;
                            timer = 0f;
                        }
                    }
                    else
                    {
                        Destroy(gameObject);
                    }
                    break;
                case BuffProjectilePhase.Shrinking:
                    timer += Time.deltaTime;
                    float tShrink = Mathf.Clamp01(timer / shrinkDuration);
                    transform.localScale = Vector3.one * Mathf.Lerp(preMoveScale, 0.3f, tShrink);
                    SetAlpha(Mathf.Lerp(1f, 0f, tShrink));
                    if (tShrink >= 1f)
                    {
                        if (target != null && buff != null)
                            target.AddBuff(buff);
                        Destroy(gameObject);
                    }
                    break;
            }
        }

        public void Init(Unit caster, Unit target, Units.Buffs.Buff buff)
        {
            this.caster = caster;
            this.target = target;
            this.buff = buff;
            var cellTypeId = buff.SourceSkill.CellTypeId;
            var sprite = cellTypeResourceMapping.GetSprite(cellTypeId);
            spriteRenderer.sprite = sprite;

            SetAlpha(0f);
            timer = 0f;
            phase = BuffProjectilePhase.Growing;
        }

        public void Activate()
        {
            isActive = true;
        }

        private void SetAlpha(float alpha)
        {
            if (spriteRenderer == null) return;
            var color = spriteRenderer.color;
            color.a = alpha;
            spriteRenderer.color = color;
        }
    }
}