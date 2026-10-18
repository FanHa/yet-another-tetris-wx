using System.Collections.Generic;
using UnityEngine;

namespace View
{
    public class TetriInventoryView : MonoBehaviour
    {
        [SerializeField] private Transform contentRoot;

        private void ClearView()
        {
            foreach (Transform child in contentRoot)
            {
                Destroy(child.gameObject);
            }
        }

        public void ShowItems(IReadOnlyList<GameObject> items)
        {
            ClearView();
            for (int i = 0; i < items.Count; i++)
            {
                var tetriObject = items[i];
                tetriObject.transform.SetParent(contentRoot, false);
                tetriObject.transform.localPosition = CalculateGridPosition(i);

                foreach (var spriteRenderer in tetriObject.GetComponentsInChildren<SpriteRenderer>(true))
                {
                    spriteRenderer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
                }
            }
        }

        private Vector3 CalculateGridPosition(int index)
        {
            const int itemsPerRow = 3;
            const float itemSize = 5f;
            const float spacing = 0.5f;
            float itemStride = itemSize + spacing;

            int row = index / itemsPerRow;
            int column = index % itemsPerRow;

            return new Vector3(column * itemStride, -row * itemStride, 0f);
        }
    }
}