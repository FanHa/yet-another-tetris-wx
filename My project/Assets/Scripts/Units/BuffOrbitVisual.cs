using System.Collections.Generic;
using Model.Resonance;
using UnityEngine;

namespace Units
{
    public sealed class BuffOrbitVisual : BuffVisual
    {
        [SerializeField] private ResonanceDatabase resonanceDatabase;
        [SerializeField] private SpriteRenderer[] particles;
        [SerializeField] private Vector2 orbitRadius = new Vector2(0.35f, 0.12f);
        [SerializeField] private Vector2 centerOffset;
        [SerializeField] private float angularSpeed = 180f;
        [SerializeField, Range(0f, 0.3f)] private float depthScaleVariation = 0.12f;

        private Unit owner;
        private Vector3[] originalScales;
        private float angle;
        private bool isActive;

        public override void Initialize(Unit owner, ResonanceType resonance)
        {
            this.owner = owner;
            isActive = false;
            angle = Random.Range(0f, 360f);
            originalScales = new Vector3[particles.Length];

            Color? resonanceColor = resonance == ResonanceType.None
                ? null
                : resonanceDatabase.GetDefinition(resonance).BaseColor;
            for (int index = 0; index < particles.Length; index++)
            {
                if (resonanceColor.HasValue)
                {
                    particles[index].color = resonanceColor.Value;
                }
                originalScales[index] = particles[index].transform.localScale;
            }
        }

        public override void Activate()
        {
            isActive = true;
            UpdateOrbit();
            for (int index = 0; index < particles.Length; index++)
            {
                particles[index].enabled = true;
            }
        }

        public override void Release()
        {
            isActive = false;
            gameObject.SetActive(false);
            Destroy(gameObject);
        }

        private void LateUpdate()
        {
            if (!isActive)
            {
                return;
            }

            angle = Mathf.Repeat(angle + angularSpeed * Time.deltaTime, 360f);
            UpdateOrbit();
        }

        private void UpdateOrbit()
        {
            transform.position = owner.transform.position + new Vector3(centerOffset.x, centerOffset.y, 0f);

            int backOrder = Mathf.Min(owner.BodySpriteRenderer.sortingOrder,
                Mathf.Min(owner.Fist1SpriteRenderer.sortingOrder, owner.Fist2SpriteRenderer.sortingOrder)) - 1;
            int frontOrder = Mathf.Max(owner.BodySpriteRenderer.sortingOrder,
                Mathf.Max(owner.Fist1SpriteRenderer.sortingOrder, owner.Fist2SpriteRenderer.sortingOrder)) + 1;

            float phaseStep = 360f / particles.Length;
            for (int index = 0; index < particles.Length; index++)
            {
                float phase = (angle + phaseStep * index) * Mathf.Deg2Rad;
                float depth = Mathf.Sin(phase);
                SpriteRenderer particle = particles[index];
                particle.transform.localPosition = new Vector3(orbitRadius.x * Mathf.Cos(phase), orbitRadius.y * depth, 0f);
                particle.transform.localScale = originalScales[index] * (1f - depth * depthScaleVariation);
                particle.sortingLayerID = owner.BodySpriteRenderer.sortingLayerID;
                particle.sortingOrder = depth > 0f ? backOrder : frontOrder;
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (resonanceDatabase == null)
            {
                Debug.LogError($"{nameof(resonanceDatabase)} is not assigned.", this);
                return;
            }


            if (!IsFinite(orbitRadius.x) || !IsFinite(orbitRadius.y) || orbitRadius.x <= 0f || orbitRadius.y <= 0f)
            {
                Debug.LogError($"{nameof(orbitRadius)} must have positive, finite components.", this);
            }

            if (!IsFinite(centerOffset.x) || !IsFinite(centerOffset.y))
            {
                Debug.LogError($"{nameof(centerOffset)} must have finite components.", this);
            }

            if (!IsFinite(angularSpeed) || angularSpeed == 0f)
            {
                Debug.LogError($"{nameof(angularSpeed)} must be nonzero and finite.", this);
            }

            if (!IsFinite(depthScaleVariation) || depthScaleVariation < 0f || depthScaleVariation > 0.3f)
            {
                Debug.LogError($"{nameof(depthScaleVariation)} must be between 0 and 0.3 and finite.", this);
            }

            if (particles == null || particles.Length == 0)
            {
                Debug.LogError($"{nameof(particles)} must contain at least one sprite renderer.", this);
                return;
            }

            var seenParticles = new HashSet<SpriteRenderer>();
            for (int index = 0; index < particles.Length; index++)
            {
                if (particles[index] == null)
                {
                    Debug.LogError($"{nameof(particles)}[{index}] is not assigned.", this);
                }
                else if (!seenParticles.Add(particles[index]))
                {
                    Debug.LogError($"{nameof(particles)}[{index}] duplicates another sprite renderer.", this);
                }
                else if (particles[index].transform.parent != transform)
                {
                    Debug.LogError($"{nameof(particles)}[{index}] must be a direct child of the orbit visual.", this);
                }
            }
        }

        private static bool IsFinite(float value)
        {
            return !float.IsNaN(value) && !float.IsInfinity(value);
        }
#endif
    }
}