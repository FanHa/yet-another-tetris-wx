using System.Collections.Generic;
using UnityEngine;
using UI;
using Controller;
using Model.Tetri;
using UI.Resource;
using Model;
using System.Collections;
using UnityEngine.UI;
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
    private Controller.Commands.CommandManager commandManager = new(); // 添加命令管理器

    [SerializeField] private Button revokeOperationButton;
    [SerializeField] private Button battleButton;
    [SerializeField] private Button unitPreviewButton;
    [SerializeField] private Button trainGroundButton;

    
    private void Start()
    {
        // 初始化资源面板和操作表
        InitializeResourcesPanel();
        InitializeOperationTable();
        InitializeBattleField();
        levelConfig.Reset();
        // 绑定撤销操作按钮的点击事件
        revokeOperationButton.onClick.AddListener(UndoLastPlacement);
        battleButton.onClick.AddListener(HandleBattleClicked);
        unitPreviewButton.onClick.AddListener(HandleUnitPreviewClicked);
        trainGroundButton.onClick.AddListener(HandleTrainGroundClicked);
    }
    

    private void InitializeBattleField()
    {
        battleField.OnBattleEnd += HandleBattleEnd;

    }

    private void HandleBattleEnd()
    {
        reward.EnterRewardSelectionPhase();
        reward.OnRewardSelected += HandleRewardSelected;
    }

    private void HandleRewardSelected()
    {
        reward.OnRewardSelected -= HandleRewardSelected;
        Camera.main.transform.position = new Vector3(0, 0, -10); // todo magic num

        commandManager.ClearHistory();
        operationTableData.ClearHistory(); // 添加清空 OperationTable 的历史记录
        tetriResource.ClearHistory(); 

        tetriResource.PrepareNewRound();
        inventory.Hide();
        levelConfig.AdvanceToNextLevel(); // 关卡增加

    }


    private void HandleTetriDrop(ItemSlot item, Vector2Int position)
    {
        var placeCommand = new Controller.Commands.PlaceTetri(operationTableData, item, position, tetriResource);

        // 执行放置命令
        commandManager.ExecuteCommand(placeCommand);
        assemblyMouseFollower.StopFollowing();
    }

    public void UndoLastPlacement()
    {
        commandManager.Undo(); // 调用命令管理器的撤销方法
    }

    private IEnumerator DelayedUseTetri(ItemSlot item)
    {
        yield return new WaitForEndOfFrame(); // 等待当前帧结束
        tetriResource.UseTetri(item);
    }

    private void HandleTetriBeginDrag(ItemSlot item)
    {
        assemblyMouseFollower.SetFollowItem(item);
        assemblyMouseFollower.StartFollowing();
    }

    private void HandleTetriEndDrag(ItemSlot item)
    {
        assemblyMouseFollower.StopFollowing();
    }

    private void InitializeResourcesPanel()
    {
        tetriResource.Initialize();
        tetriResource.OnTetriBegainDrag += HandleTetriBeginDrag;
        tetriResource.OnTetriEndDrag += HandleTetriEndDrag;
    }

    private void InitializeOperationTable()
    {
        
        // TODO delete magic number
        operationTableData.Init(10, 10);
        operationTableData.OnTableChanged += UpdateOperationTableUI;
        operationTableUI.OnTetriDrop += HandleTetriDrop;
        UpdateOperationTableUI();
    }

    private void HandleBattleClicked()
    {
        LoadLevelData();
        GenerateAndResetInventoryData();
        Camera.main.transform.position = new Vector3(battleField.transform.position.x, battleField.transform.position.y, Camera.main.transform.position.z);                
        battleField.StartNewLevelBattle(levelConfig.currentLevel);

    }

    private void HandleTrainGroundClicked()
    {
        Camera.main.transform.position = new Vector3(battleField.transform.position.x, battleField.transform.position.y, Camera.main.transform.position.z);                
        battleField.StartTrainGroundBattle();
    }


    private void LoadLevelData()
    {
        List<InventoryItem> levelData = levelConfig.GetEnemyData(); // 获取当前关卡数据
        battleField.SetEnemyData(levelData); // Pass enemy data to BattleField
    }

    private void HandleUnitPreviewClicked()
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
        List<List<Model.Tetri.Cell>> fullRows = operationTableData.GetCharacterCellGroups();
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
