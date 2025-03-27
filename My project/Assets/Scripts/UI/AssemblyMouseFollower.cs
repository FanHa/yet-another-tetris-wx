using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class AssemblyMouseFollower : MonoBehaviour
    {
        [SerializeField] private Canvas canvas; // 画布
        [SerializeField] private OperationTable operationTable; // 替换Tilemap为OperationTable
        [SerializeField] private Controller.Tetris tetrisFactory;
        private bool isFollowing = false; // 开关，控制图像是否跟随鼠标

        // Start is called before the first frame update
        void Start()
        {

            transform.gameObject.SetActive(false); // 初始时隐藏图像
        }

        // Update is called once per frame
        void Update()
        {
            if (!isFollowing) return;

            // 将物体移动到鼠标所在位置
            Vector2 mouseLocalPosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                Input.mousePosition,
                canvas.worldCamera,
                out mouseLocalPosition
            );
            transform.position = canvas.transform.TransformPoint(mouseLocalPosition);

            // 如果鼠标在 OperationTable 上，调整位置到对应单元格中心
            if (RectTransformUtility.RectangleContainsScreenPoint(
                    operationTable.GetComponent<RectTransform>(),
                    Input.mousePosition,
                    canvas.worldCamera))
            {
                Vector2 operationTableLocalPoint;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    operationTable.GetComponent<RectTransform>(),
                    Input.mousePosition,
                    canvas.worldCamera,
                    out operationTableLocalPoint
                );

                // 计算单元格位置
                Vector2Int cellPosition = new Vector2Int(
                    Mathf.FloorToInt(operationTableLocalPoint.x / operationTable.CellSize.x),
                    Mathf.FloorToInt(operationTableLocalPoint.y / operationTable.CellSize.y)
                );

                // 获取单元格中心位置并更新物体位置
                Vector3 cellCenterPosition = operationTable.GetCellCenterWorld(cellPosition);
                transform.position = cellCenterPosition;
            }

        }

        // 设置要跟随鼠标的图像
        public void SetFollowItem(UI.Resource.ItemSlot item)
        {
            // 清理原来设置的item
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
            GameObject shadow = tetrisFactory.GenerateTetriPreview(item.GetTetri(), new Vector2(100,100)); // todo magic number
            shadow.transform.SetParent(transform, false);
            
        }

        // 开启跟随
        public void StartFollowing()
        {
            isFollowing = true;
            transform.gameObject.SetActive(true);

        }

        // 关闭跟随
        public void StopFollowing()
        {
            isFollowing = false;
            transform.gameObject.SetActive(false);
        }
    }
}