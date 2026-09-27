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
        private bool isValidated;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (affinityDisplayConfig == null)
            {
                Debug.LogError($"{nameof(affinityDisplayConfig)} is not assigned.", this);
            }
        }
#endif

        public void Init(Model.Tetri.Cell modelCell)
        {
            if (modelCell == null)
            {
                throw new ArgumentNullException(nameof(modelCell));
            }

            EnsureValidated();

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
            EnsureValidated();
            borderTop.SetActive(top);
            borderBottom.SetActive(bottom);
            borderLeft.SetActive(left);
            borderRight.SetActive(right);
        }

        private void EnsureValidated()
        {
            if (isValidated)
            {
                return;
            }

            if (mask == null) throw new InvalidOperationException("Cell.mask is not assigned.");
            if (icon == null) throw new InvalidOperationException("Cell.icon is not assigned.");
            if (borderTop == null) throw new InvalidOperationException("Cell.borderTop is not assigned.");
            if (borderBottom == null) throw new InvalidOperationException("Cell.borderBottom is not assigned.");
            if (borderLeft == null) throw new InvalidOperationException("Cell.borderLeft is not assigned.");
            if (borderRight == null) throw new InvalidOperationException("Cell.borderRight is not assigned.");
            maskRenderer = mask.GetComponent<SpriteRenderer>()
                ?? throw new InvalidOperationException("Cell.mask missing SpriteRenderer.");
            borderTopRenderer = borderTop.GetComponent<SpriteRenderer>()
                ?? throw new InvalidOperationException("Cell.borderTop missing SpriteRenderer.");
            borderBottomRenderer = borderBottom.GetComponent<SpriteRenderer>()
                ?? throw new InvalidOperationException("Cell.borderBottom missing SpriteRenderer.");
            borderLeftRenderer = borderLeft.GetComponent<SpriteRenderer>()
                ?? throw new InvalidOperationException("Cell.borderLeft missing SpriteRenderer.");
            borderRightRenderer = borderRight.GetComponent<SpriteRenderer>()
                ?? throw new InvalidOperationException("Cell.borderRight missing SpriteRenderer.");

            isValidated = true;
        }

    }
}