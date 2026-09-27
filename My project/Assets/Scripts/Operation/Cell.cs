using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Operation
{
    public class Cell : MonoBehaviour
    {
        [Header("Visual Components")]
        [SerializeField] private GameObject mask;
        [SerializeField] private SpriteRenderer icon;
        [SerializeField] private GameObject borderTop;
        [SerializeField] private GameObject borderBottom;
        [SerializeField] private GameObject borderLeft;
        [SerializeField] private GameObject borderRight;
        [SerializeField, FormerlySerializedAs("colorConfig")]
        private Model.Tetri.AffinityDisplayConfig affinityDisplayConfig;

        private SpriteRenderer maskRenderer;
        private SpriteRenderer borderTopRenderer;
        private SpriteRenderer borderBottomRenderer;
        private SpriteRenderer borderLeftRenderer;
        private SpriteRenderer borderRightRenderer;

        private void Awake()
        {
            maskRenderer = mask.GetComponent<SpriteRenderer>();
            borderTopRenderer = borderTop.GetComponent<SpriteRenderer>();
            borderBottomRenderer = borderBottom.GetComponent<SpriteRenderer>();
            borderLeftRenderer = borderLeft.GetComponent<SpriteRenderer>();
            borderRightRenderer = borderRight.GetComponent<SpriteRenderer>();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            ValidateRenderer(icon, nameof(icon));
            ValidateRenderer(mask, nameof(mask));
            ValidateRenderer(borderTop, nameof(borderTop));
            ValidateRenderer(borderBottom, nameof(borderBottom));
            ValidateRenderer(borderLeft, nameof(borderLeft));
            ValidateRenderer(borderRight, nameof(borderRight));
            ValidateReference(affinityDisplayConfig, nameof(affinityDisplayConfig));
        }

        private void ValidateRenderer(GameObject target, string fieldName)
        {
            if (target == null)
            {
                Debug.LogError($"{fieldName} is not assigned.", this);
                return;
            }

            if (!target.TryGetComponent(out SpriteRenderer _))
            {
                Debug.LogError($"{fieldName} is missing a SpriteRenderer.", this);
            }
        }

        private void ValidateRenderer(SpriteRenderer target, string fieldName)
        {
            ValidateReference(target, fieldName);
        }

        private void ValidateReference(UnityEngine.Object target, string fieldName)
        {
            if (target == null)
            {
                Debug.LogError($"{fieldName} is not assigned.", this);
            }
        }
#endif

        public void Init(Model.Tetri.Cell modelCell)
        {
            if (modelCell == null)
            {
                throw new ArgumentNullException(nameof(modelCell));
            }

            icon.sprite = modelCell.Icon;

            var displayEntry = affinityDisplayConfig.GetDisplayEntry(modelCell.Affinity)
                ?? throw new InvalidOperationException($"Missing affinity display config for '{modelCell.Affinity}'.");

            SetMaskColor(displayEntry.maskColor);
            SetBorderColor(displayEntry.borderColor);

        }

        private void SetMaskColor(Color color)
        {
            maskRenderer.color = color;
        }

        private void SetBorderColor(Color color)
        {
            borderTopRenderer.color = color;
            borderBottomRenderer.color = color;
            borderLeftRenderer.color = color;
            borderRightRenderer.color = color;
        }

        public void SetBorderVisibility(bool top, bool bottom, bool left, bool right)
        {
            borderTop.SetActive(top);
            borderBottom.SetActive(bottom);
            borderLeft.SetActive(left);
            borderRight.SetActive(right);
        }

    }
}