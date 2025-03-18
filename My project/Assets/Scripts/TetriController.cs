using System.Collections.Generic;
using UnityEngine;
using UI;
using Controller;
using UI.TetrisResource;
using Model.Tetri;
public class TetriController : MonoBehaviour
{
    [SerializeField] private UI.OperationTable operationTableUI;
    [SerializeField] private Model.OperationTable operationTableData;
    [SerializeField] private AssemblyMouseFollower assemblyMouseFollower;
    private TetriResource tetriResource;
    private Scene scene;
    private BattleField battleField;
    private Controller.Reward reward;

    private Controller.Inventory inventory;

    private UI.TetrisResource.TetrisResourceItem currentDraggingTetri; // 保存当前拖动的Tetri

    private void Awake()
    {
        // 获取 Scene 和 BattleField 的引用
        scene = GetComponent<Scene>();
        battleField = GetComponent<BattleField>();
        inventory = GetComponent<Controller.Inventory>();
        reward = GetComponent<Controller.Reward>();
        tetriResource = GetComponent<TetriResource>();

        if (scene == null)
        {
            Debug.LogWarning("Scene component not found on the same GameObject.");
        }

        if (battleField == null)
        {
            Debug.LogWarning("BattleField component not found on the same GameObject.");
        }
    }

    private void Start()
    {
        // 初始化资源面板和操作表
        InitializeResourcesPanel();
        InitializeOperationTable();
        InitializeSceneMonitor();
        InitializeBattleField();
    }

    private void InitializeBattleField()
    {
        battleField.OnFactionDefeated += HandleFactionDefeated;

    }

    private void HandleFactionDefeated(Units.Unit.Faction faction)
    {
        scene.SwitchToOperationPhase();
        reward.EnterRewardSelectionPhase();
        reward.OnRewardSelected += HandleRewardSelected;
    }

    private void HandleRewardSelected()
    {
        reward.OnRewardSelected -= HandleRewardSelected;
        // 继续游戏逻辑
        Debug.Log("Reward selected: " + reward);
    }

    private void InitializeSceneMonitor()
    {
        scene.OnSwitchToOperationPhase += HandleSwitchToOperationPhase;
    }

    private void HandleSwitchToOperationPhase()
    {
        battleField.DestroyAllUnits();
        tetriResource.PrepareNewRound();
    }

    private void HandleTetriDropped(TetrisResourceItem item, Vector3Int position)
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

    private void HandleTetriBeginDrag(TetrisResourceItem item)
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
        
        operationTableUI.OnTetriDropped += HandleTetriDropped;
        // TODO delete magic number
        operationTableData.Init(10, 10);
        operationTableData.OnTableChanged += UpdateOperationTableUI;

        UpdateOperationTableUI();
    }

    public void HandleBattleClicked()
    {
        GenerateAndResetInventoryData();
        scene.SwitchToBattlePhase();
    }

    public void HandleInventoryClicked()
    {
        bool isOpend = inventory.ToggleInventory();
        if (isOpend)
        {
            GenerateAndResetInventoryData();
        }
    }

    private void GenerateAndResetInventoryData()
    {
        List<Model.InventoryItem> items = new List<Model.InventoryItem>();
        List<List<TetriCell>> fullRows = operationTableData.GetFullRows();
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
        operationTableUI.OnTetriDropped -= HandleTetriDropped;
    }
}
