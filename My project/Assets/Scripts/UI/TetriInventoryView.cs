using System;
using System.Collections.Generic;
using Model.Tetri;
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

                foreach (var sr in tetriObject.GetComponentsInChildren<SpriteRenderer>(true))
                {
                    sr.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
                }
            
            }
        }

        private Vector3 CalculateGridPosition(int index)
        {
            int perRow = 3;
            float size = 5f;
            float spacing = 0.5f;
            float cellSize = size + spacing;

            int row = index / perRow;
            int col = index % perRow;

            return new Vector3(col * cellSize, -row * cellSize, 0);
        }
    }
}