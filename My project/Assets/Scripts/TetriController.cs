using System.Collections.Generic;
using UnityEngine;
using UI;
using Controller;
using Model.Tetri;
using UI.Resource;
using Model;
public class TetriController : MonoBehaviour
{
    [SerializeField] private UI.OperationTable operationTableUI;
    [SerializeField] private Model.OperationTable operationTableData;
    [SerializeField] private AssemblyMouseFollower assemblyMouseFollower;
    [SerializeField] private BattleField battleField;
    [SerializeField] private Controller.Resource tetriResource;
    [SerializeField] private Controller.Reward reward;

    [SerializeField] private Controller.Inventory inventory;
    [SerializeField] private Model.LevelConfig levelConfig; // 关卡配置
    

    private UI.Resource.ItemSlot currentDraggingTetri; // 保存当前拖动的Tetri

    private void Start()
    {
        // 初始化资源面板和操作表
        InitializeResourcesPanel();
        InitializeOperationTable();
        InitializeBattleField();
        levelConfig.Reset();
    }

    private void InitializeBattleField()
    {
        battleField.OnFactionDefeated += HandleFactionDefeated;

    }

    private void HandleFactionDefeated()
    {
        reward.EnterRewardSelectionPhase();
        reward.OnRewardSelected += HandleRewardSelected;
    }

    private void HandleRewardSelected()
    {
        reward.OnRewardSelected -= HandleRewardSelected;
        Camera.main.transform.position = new Vector3(0, 0, -10); // todo magic num

        battleField.DestroyAllUnits();
        tetriResource.PrepareNewRound();
        inventory.Hide();
        levelConfig.AdvanceToNextLevel(); // 关卡增加

    }


    private void HandleTetriDrop(ItemSlot item, Vector2Int position)
    {
        // 1. 调用OperationTableSO的方法设置一个新的Tetri
        bool isPlaced = operationTableData.PlaceTetri(new Vector2Int(position.x, position.y), item.GetTetri());

        // 2. 解除currentDraggingTetri状态
        currentDraggingTetri = default;
        assemblyMouseFollower.StopFollowing();

        if (isPlaced)
        {
            tetriResource.UseTetri(item);

        }
    }

    private void HandleTetriBeginDrag(ItemSlot item)
    {
        // 保存当前拖动的Tetri信息
        currentDraggingTetri = item;
        assemblyMouseFollower.SetFollowItem(item);
        assemblyMouseFollower.StartFollowing();
    }

    private void InitializeResourcesPanel()
    {
        tetriResource.Initialize();
        tetriResource.OnTetriBegainDrag += HandleTetriBeginDrag;
    }

    private void InitializeOperationTable()
    {
        
        // TODO delete magic number
        operationTableData.Init(10, 10);
        operationTableData.OnTableChanged += UpdateOperationTableUI;
        operationTableUI.OnTetriDrop += HandleTetriDrop;
        UpdateOperationTableUI();
    }

    public void HandleBattleClicked()
    {
        LoadLevelData();
        GenerateAndResetInventoryData();
        Camera.main.transform.position = new Vector3(battleField.transform.position.x, battleField.transform.position.y, Camera.main.transform.position.z);                
        battleField.StartNewLevelBattle(levelConfig.currentLevel);

    }

    private void LoadLevelData()
    {
        List<InventoryItem> levelData = levelConfig.GetEnemyData(); // 获取当前关卡数据
        battleField.SetEnemyData(levelData); // Pass enemy data to BattleField
    }

    public void HandleInventoryClicked()
    {
        bool isOpend = inventory.ToggleInventory();
        if (isOpend)
        {
            operationTableUI.gameObject.SetActive(false);
            GenerateAndResetInventoryData();
        } else {
            operationTableUI.gameObject.SetActive(true);
        }
    }

    private void GenerateAndResetInventoryData()
    {
        List<Model.InventoryItem> items = new List<Model.InventoryItem>();
        List<List<Cell>> fullRows = operationTableData.GetFullRows();
        foreach (var rowCells in fullRows)
        {
            Model.InventoryItem item = inventory.GenerateInventoryItemFromTetriCells(rowCells);
            items.Add(item);
        }
        inventory.ResetInventoryData(items);
    }

    private void UpdateOperationTableUI()
    {
        // 更新操作表UI
        operationTableUI.UpdateData(operationTableData.GetBoardData());
    }

    private void OnDestroy()
    {
        // 取消监听UI的事件
        operationTableUI.OnTetriDrop -= HandleTetriDrop;
    }
}
