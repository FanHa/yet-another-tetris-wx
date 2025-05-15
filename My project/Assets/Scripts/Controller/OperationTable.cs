using UnityEngine;

namespace Controller
{

    public class OperationTableCreator : MonoBehaviour
    {
        [Header("格子预制体")]
        [SerializeField] private GameObject cellPrefab;

        [Header("容器大小")]
        [SerializeField] private int width;
        [SerializeField] private int height;

        [Header("每个格子的间距")]
        [SerializeField] private float cellSize;

        private void Start()
        {
            CreateGrid();
        }

        private void CreateGrid()
        {
            Vector2 origin = new Vector2(-width / 2f + 0.5f, -height / 2f + 0.5f);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Vector2 pos = origin + new Vector2(x * cellSize, y * cellSize);
                    GameObject cell = Instantiate(cellPrefab, pos, Quaternion.identity, transform);
                    cell.name = $"Cell_{x}_{y}";
                }
            }
        }
    }

}