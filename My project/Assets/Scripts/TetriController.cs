using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI;
using Model;
using System;

public class TetriController : MonoBehaviour
{
    [SerializeField] private TetrisResourcesSO tetrisResourcesSO;
    [SerializeField] private TetrisResourcePanel tetrisResourcePanelUI;
    [SerializeField] private OperationTable operationTableUI;
    [SerializeField] private OperationTableSO operationTableSO;

    private Model.Tetri currentDraggingTetri; // 保存当前拖动的Tetri

    private void Start()
    {
        // 初始化资源面板和操作表
        InitializeResourcesPanel();
        InitializeOperationTable();
    }

    private void HandleTetriDropped(TetrisResourceItem item, Vector3Int position)
    {
        // 处理TetrisResourceItem放置事件
        Debug.Log($"TetrisResourceItem dropped at position: {position}");

        // 1. 调用OperationTableSO的方法设置一个新的Tetri
        operationTableSO.PlaceTetri(new Vector2Int(position.x, position.y), item.GetTetri());
        // todo 判断PlaceTetri是否成功

        // 2. 解除currentDraggingTetri状态
        currentDraggingTetri = default;

        // 3. 调用tetrisResourcesSO告诉它一个tetri已被移出了
        tetrisResourcesSO.RemoveTetri(item.GetTetri());
    }

    private void HandleTetriBeginDrag(Tetri tetri)
    {
        // 保存当前拖动的Tetri信息
        currentDraggingTetri = tetri;
        Debug.Log($"Tetri begin drag: {tetri}");

        // 调用TetrisResourcesSO的方法设置某个Tetri被拖动
        tetrisResourcesSO.SetTetriDragged(tetri);
    }

    private void InitializeResourcesPanel()
    {
        // 初始化资源面板
        tetrisResourcesSO.OnDataChanged += UpdateResourcesPanelUI;
        tetrisResourcePanelUI.OnTetriBeginDrag += HandleTetriBeginDrag;
        AddTestData();

    }

    private void InitializeOperationTable()
    {
        
        operationTableSO.OnTableChanged += UpdateOperationTableUI;
        operationTableUI.OnTetriDropped += HandleTetriDropped;

        operationTableSO.Init(10, 10); // 假设操作表大小为10x10
        operationTableUI.UpdateData(operationTableSO.GetBoardData());
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
        // 添加测试数据
        List<Tetri> testTetriList = new List<Tetri>
        {
            new Tetri(new int[,] 
            { 
                { 1, 1, 0, 0 }, 
                { 1, 1, 0, 0 }, 
                { 0, 0, 0, 0 }, 
                { 0, 0, 0, 0 } 
            }),
            new Tetri(new int[,] 
            { 
                { 1, 1, 1, 0 }, 
                { 0, 1, 0, 0 }, 
                { 0, 0, 0, 0 }, 
                { 0, 0, 0, 0 } 
            }),
            new Tetri(new int[,] 
            { 
                { 1, 0, 0, 0 }, 
                { 1, 1, 0, 0 }, 
                { 1, 0, 0, 0 }, 
                { 0, 0, 0, 0 } 
            })
        };
        tetrisResourcesSO.AddTetriRange(testTetriList);
    }

    private void OnDestroy()
    {
        // 取消监听SO数据变化
        operationTableSO.OnTableChanged -= UpdateOperationTableUI;

        // 取消监听UI的事件
        operationTableUI.OnTetriDropped -= HandleTetriDropped;
    }
}
