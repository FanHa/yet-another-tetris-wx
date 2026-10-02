using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Units.Skills;

namespace Units.Projectiles
{
    public class ChainLightning : MonoBehaviour
    {
        [Header("开发调校")]
        [SerializeField, Min(0.01f)] private float travelSpeed = 18f;
        [SerializeField, Min(0f)] private float bounceDelay = 0.2f;
        [SerializeField, Min(0f)] private float lifetime = 0.5f;

        [Header("表现")]
        [SerializeField] private Texture[] textures;
        [SerializeField] private float fps;

        private LineRenderer lineRenderer;
        private int animationStep;
        private float fpsCounter;
        private Unit caster;
        private Skill sourceSkill;
        private Unit initialTarget;
        private float currentDamage;
        private float damageIncreasePercentage;
        private int maxTargets;
        private float chainRange;
        private readonly HashSet<Unit> hitTargets = new();
        private bool isActive;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (travelSpeed <= 0f)
            {
                Debug.LogError($"{nameof(travelSpeed)} must be greater than 0.", this);
            }

            if (bounceDelay < 0f)
            {
                Debug.LogError($"{nameof(bounceDelay)} must not be negative.", this);
            }

            if (lifetime < 0f)
            {
                Debug.LogError($"{nameof(lifetime)} must not be negative.", this);
            }

            if (fps < 0f)
            {
                Debug.LogError($"{nameof(fps)} must not be negative.", this);
            }

            if (GetComponent<LineRenderer>() == null)
            {
                Debug.LogError($"{nameof(LineRenderer)} component is required.", this);
            }
        }
#endif

        private void Awake()
        {
            lineRenderer = GetComponent<LineRenderer>();
        }

        private void Update()
        {
            fpsCounter += Time.deltaTime;
            if (textures != null && textures.Length > 0 && fps > 0f && fpsCounter >= 1f / fps)
            {
                fpsCounter = 0f;
                animationStep = (animationStep + 1) % textures.Length;
                lineRenderer.material.mainTexture = textures[animationStep];
            }
        }

        public void Initialize(
            Unit caster,
            Unit initialTarget,
            float baseDamage,
            float damageIncreasePercentage,
            int maxTargets,
            float chainRange,
            Skill sourceSkill
        )
        {
            this.caster = caster;
            this.initialTarget = initialTarget;
            currentDamage = baseDamage;
            this.damageIncreasePercentage = damageIncreasePercentage;
            this.maxTargets = maxTargets;
            this.chainRange = chainRange;
            this.sourceSkill = sourceSkill;
        }

        public void Activate()
        {
            if (isActive)
            {
                return;
            }

            isActive = true;
            lineRenderer.positionCount = 2;
            Vector3 origin = caster.transform.position;
            lineRenderer.SetPosition(0, origin);
            lineRenderer.SetPosition(1, origin);
            StartCoroutine(RunChain());
        }

        private IEnumerator RunChain()
        {
            Unit currentTarget = initialTarget;
            Vector3 segmentStart = caster.transform.position;
            int hitCount = 0;

            while (currentTarget != null && currentTarget.IsActive && hitCount < maxTargets)
            {
                if (!hitTargets.Add(currentTarget))
                {
                    currentTarget = FindNextTarget(segmentStart);
                    continue;
                }

                yield return AnimateSegment(segmentStart, currentTarget);
                if (currentTarget == null || !currentTarget.IsActive)
                {
                    break;
                }

                Vector3 impactPosition = currentTarget.transform.position;
                ApplyDamage(currentTarget);
                hitCount++;

                if (hitCount >= maxTargets)
                {
                    break;
                }

                yield return new WaitForSeconds(bounceDelay);

                segmentStart = impactPosition;
                currentDamage += currentDamage * (damageIncreasePercentage / 100f);
                currentTarget = FindNextTarget(segmentStart);
            }

            Destroy(gameObject, lifetime);
        }

        private IEnumerator AnimateSegment(Vector3 startPosition, Unit target)
        {
            float duration = Vector3.Distance(startPosition, target.transform.position) / travelSpeed;
            float elapsed = 0f;

            while (elapsed < duration && target != null && target.IsActive)
            {
                float progress = Mathf.Clamp01(elapsed / duration);
                Vector3 headPosition = Vector3.Lerp(startPosition, target.transform.position, progress);
                lineRenderer.SetPosition(0, startPosition);
                lineRenderer.SetPosition(1, headPosition);
                elapsed += Time.deltaTime;
                yield return null;
            }

            if (target != null && target.IsActive)
            {
                lineRenderer.SetPosition(0, startPosition);
                lineRenderer.SetPosition(1, target.transform.position);
            }
        }

        private void ApplyDamage(Unit target)
        {
            var damage = new Damages.Damage(currentDamage, Damages.DamageType.Skill)
                .SetSourceLabel(sourceSkill.Name())
                .SetSourceUnit(caster)
                .SetTargetUnit(target);
            target.TakeDamage(damage);
        }

        private Unit FindNextTarget(Vector3 origin)
        {
            if (caster.UnitManager == null)
            {
                return null;
            }

            return caster.UnitManager.FindEnemiesInRangeAtPosition(
                    caster.faction,
                    (Vector2)origin,
                    chainRange
                )
                .Where(enemy => enemy != null && enemy.IsActive && !hitTargets.Contains(enemy))
                .OrderBy(enemy => ((Vector2)enemy.transform.position - (Vector2)origin).sqrMagnitude)
                .FirstOrDefault();
        }
    }
}