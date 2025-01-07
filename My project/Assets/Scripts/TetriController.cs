using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI;
using Model;
public class TetriController : MonoBehaviour
{
    [SerializeField] private TetrisResources tetrisResourcesSO;
    [SerializeField] private TetrisResourcePanel tetrisResourcePanelUI;
    [SerializeField] private OperationTable operationTableUI;
    [SerializeField] private OperationTableSO operationTableSO;
    [SerializeField] private AssemblyMouseFollower assemblyMouseFollower;
    [SerializeField] private Model.Inventory inventoryData;

    [SerializeField] private CombatUnit testUnit;

    private TetrisResourceItem currentDraggingTetri; // 保存当前拖动的Tetri

    private void Start()
    {
        // 初始化资源面板和操作表
        InitializeResourcesPanel();
        InitializeOperationTable();
    }

    private void HandleTetriDropped(TetrisResourceItem item, Vector3Int position)
    {
        // 1. 调用OperationTableSO的方法设置一个新的Tetri
        bool isPlaced = operationTableSO.PlaceTetri(new Vector2Int(position.x, position.y), item.GetTetri());

        // 2. 解除currentDraggingTetri状态
        currentDraggingTetri = default;
        assemblyMouseFollower.StopFollowing();

        // 3. 调用tetrisResourcesSO告诉它一个tetri已被移出了
        if (isPlaced)
        {
            operationTableSO.CheckAndClearFullRows();
            tetrisResourcesSO.UseTetri(item.GetTetri());
            // todo 删除测试代码

            if (tetrisResourcesSO.IsEmpty())
            {
                AddTestData();
            }

        }
    }

    private void HandleTetriBeginDrag(TetrisResourceItem item)
    {
        // 保存当前拖动的Tetri信息
        currentDraggingTetri = item;
        assemblyMouseFollower.SetFollowItem(item);
        assemblyMouseFollower.StartFollowing();

        // 调用TetrisResourcesSO的方法设置某个Tetri被拖动
        tetrisResourcesSO.SetTetriDragged(item.GetTetri());
    }

    private void InitializeResourcesPanel()
    {
        // 初始化资源面板
        tetrisResourcesSO.OnDataChanged += UpdateResourcesPanelUI;
        tetrisResourcePanelUI.OnTetriResourceItemBeginDrag += HandleTetriBeginDrag;
        AddTestData();

    }

    private void InitializeOperationTable()
    {
        
        operationTableSO.OnTableChanged += UpdateOperationTableUI;
        operationTableUI.OnTetriDropped += HandleTetriDropped;
        operationTableSO.OnRowCleared += HandleOperationTableRowCleared;

        // TODO delete magic number
        operationTableSO.Init(10, 10); // 假设操作表大小为10x10
        operationTableUI.UpdateData(operationTableSO.GetBoardData());
    }

    private void HandleOperationTableRowCleared(RowClearedInfo info)
    {
        var bricks = info.clearedBricks; //todo 创建一个工厂类,根据cleardBricks生成对应的Unit

        inventoryData.AddCombatUnit(testUnit);
    }

    private void UpdateOperationTableUI()
    {
        // 更新操作表UI
        operationTableUI.UpdateData(operationTableSO.GetBoardData());
    }

    private void UpdateResourcesPanelUI()
    {
        
        // 更新资源面板UI
        tetrisResourcePanelUI.UpdatePanel(tetrisResourcesSO.GetAllTetris());
    }

    private void AddTestData()
    {
        // tetrisResourcesSO.ResetAllItems();
        // // 添加测试数据
        // List<Tetri> testTetriList = new List<Tetri>
        // {
        //     new Tetri(new int[,] 
        //     { 
        //         { 1, 1, 0, 0 }, 
        //         { 1, 1, 0, 0 }, 
        //         { 0, 0, 0, 0 }, 
        //         { 0, 0, 0, 0 } 
        //     }),
        //     new Tetri(new int[,] 
        //     { 
        //         { 1, 1, 1, 0 }, 
        //         { 0, 1, 0, 0 }, 
        //         { 0, 0, 0, 0 }, 
        //         { 0, 0, 0, 0 } 
        //     }),
        //     new Tetri(new int[,] 
        //     { 
        //         { 1, 0, 0, 0 }, 
        //         { 1, 1, 0, 0 }, 
        //         { 1, 0, 0, 0 }, 
        //         { 0, 0, 0, 0 } 
        //     })
        // };
        // tetrisResourcesSO.AddUnusedTetriRange(testTetriList);
    }

    private void OnDestroy()
    {
        // 取消监听SO数据变化
        operationTableSO.OnTableChanged -= UpdateOperationTableUI;

        // 取消监听UI的事件
        operationTableUI.OnTetriDropped -= HandleTetriDropped;
    }
}
