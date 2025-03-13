using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI;
using Model;
using Controller;
using UI.TetrisResource;
using UI.Reward;
using UnityEditor.Search;
using Model.Tetri;
public class TetriController : MonoBehaviour
{
    [SerializeField] private TetrisResources tetrisResourcesData;
    [SerializeField] private UI.TetrisResource.TetrisResourcePanel tetrisResourcePanelUI;
    [SerializeField] private UI.OperationTable operationTableUI;
    [SerializeField] private Model.OperationTable operationTableData;
    [SerializeField] private AssemblyMouseFollower assemblyMouseFollower;
    [SerializeField] private Panel rewardPanel;

    [SerializeField] private TetrisListTemplate tetrisListTemplate;
    private Scene scene;
    private BattleField battleField;

    private Controller.Inventory inventory;

    private UI.TetrisResource.TetrisResourceItem currentDraggingTetri; // 保存当前拖动的Tetri

    private void Awake()
    {
        // 获取 Scene 和 BattleField 的引用
        scene = GetComponent<Scene>();
        battleField = GetComponent<BattleField>();
        inventory = GetComponent<Controller.Inventory>();

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
        ShowRewardPanel(new List<string> { "Reward1", "Reward2", "Reward3" }); // 示例奖励列表
    }

    private void ShowRewardPanel(List<string> rewards)
    {
        rewardPanel.gameObject.SetActive(true);
        rewardPanel.SetRewards(rewards);
        rewardPanel.OnRewardSelected += HandleRewardSelected;
    }

    private void HandleRewardSelected(string reward)
    {
        rewardPanel.OnRewardSelected -= HandleRewardSelected;
        rewardPanel.gameObject.SetActive(false);
        // 继续游戏逻辑
        Debug.Log("Reward selected: " + reward);
        // ...这里可以添加更多逻辑，例如通知外部controller...
    }

    private void InitializeSceneMonitor()
    {
        scene.OnSwitchToOperationPhase += HandleSwitchToOperationPhase;
    }

    private void HandleSwitchToOperationPhase()
    {
        battleField.DestroyAllUnits();
        tetrisResourcesData.DrawRandomTetriFromUnusedList(3);
    }

    private void HandleTetriDropped(TetrisResourceItem item, Vector3Int position)
    {
        // 1. 调用OperationTableSO的方法设置一个新的Tetri
        bool isPlaced = operationTableData.PlaceTetri(new Vector2Int(position.x, position.y), item.GetTetri());

        // 2. 解除currentDraggingTetri状态
        currentDraggingTetri = default;
        assemblyMouseFollower.StopFollowing();

        // 3. 调用tetrisResourcesSO告诉它一个tetri已被移出了
        if (isPlaced)
        {
            // operationTableData.CheckAndClearFullRows();
            tetrisResourcesData.UseTetri(item.GetTetri());

        }
    }

    private void HandleTetriBeginDrag(TetrisResourceItem item)
    {
        // 保存当前拖动的Tetri信息
        currentDraggingTetri = item;
        assemblyMouseFollower.SetFollowItem(item);
        assemblyMouseFollower.StartFollowing();

        // 调用TetrisResourcesSO的方法设置某个Tetri被拖动
        tetrisResourcesData.SetTetriDragged(item.GetTetri());
    }

    private void InitializeResourcesPanel()
    {
        // 初始化资源面板
        tetrisResourcePanelUI.OnTetriResourceItemBeginDrag += HandleTetriBeginDrag;
        tetrisResourcesData.OnDataChanged += UpdateResourcesPanelUI;
        tetrisResourcesData.Reset();
        tetrisResourcesData.InitialUnusedTetris(tetrisListTemplate.template);
        tetrisResourcesData.DrawRandomTetriFromUnusedList(6);
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
        GenerateAndResetInventoryData();
        inventory.ToggleInventory();
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

    private void UpdateResourcesPanelUI()
    {
        
        // 更新资源面板UI
        tetrisResourcePanelUI.UpdatePanels(
            tetrisResourcesData.GetUsableTetris(),
            tetrisResourcesData.GetUsedTetris(),
            tetrisResourcesData.GetUnusedTetris());
    }

    private void OnDestroy()
    {
        // 取消监听UI的事件
        operationTableUI.OnTetriDropped -= HandleTetriDropped;
    }
}
