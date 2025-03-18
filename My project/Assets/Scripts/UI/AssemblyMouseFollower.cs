using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

namespace UI
{
    public class AssemblyMouseFollower : MonoBehaviour
    {
        [SerializeField] private Canvas canvas; // 画布
        [SerializeField] private Tilemap operationTableTilemap; 
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
            if (isFollowing)
            {
                Vector2 position;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    canvas.transform as RectTransform, 
                    Input.mousePosition, 
                    canvas.worldCamera, 
                    out position);
                Vector3 worldPosition = canvas.transform.TransformPoint(position);

                // 判断鼠标是否在OperationTable的Tile上
                Vector3Int cellPosition = operationTableTilemap.WorldToCell(worldPosition);

                // 将位置调整为格子的中心
                Vector3 cellCenterPosition = operationTableTilemap.GetCellCenterWorld(cellPosition);
                transform.position = cellCenterPosition;

            }
        }

        // 设置要跟随鼠标的图像
        public void SetFollowItem(UI.TetrisResource.TetrisResourceItem item)
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