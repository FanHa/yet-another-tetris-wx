using System.Collections.Generic;
using UnityEngine;
using UI;
using Controller;
using Model.Tetri;
using UI.Resource;
using Model;
using System.Collections;
using UnityEngine.UI;
using System;
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

    [SerializeField] private GameObject tetriPrefab;
    [SerializeField] private Controller.TetriInventoryController tetriInventoryController;
    [SerializeField] private Controller.OperationTableController operationTableController;
    private GameObject currentShadowTetri;
    private Operation.TetriFactory tetriFactory;

    private void Start()
    {
        // 初始化资源面板和操作表
        InitializeResourcesPanel();
        InitializeOperationTable();
        battleField.OnBattleEnd += HandleBattleEnd;
        levelConfig.Reset();
        // 绑定撤销操作按钮的点击事件
        revokeOperationButton.onClick.AddListener(UndoLastPlacement);
        battleButton.onClick.AddListener(HandleBattleClicked);
        unitPreviewButton.onClick.AddListener(HandleUnitPreviewClicked);
        trainGroundButton.onClick.AddListener(HandleTrainGroundClicked);

        tetriInventoryController.OnTetriBeginDrag += HandleTetriBeginDrag;
        tetriFactory = new Operation.TetriFactory(tetriPrefab);
    }

    private void HandleTetriBeginDrag(Operation.Tetri tetri)
    {
        var operationTetri = tetriFactory.CreateTetri(tetri.ModelTetri);
        currentShadowTetri = operationTetri.gameObject;
        currentShadowTetri.transform.position = tetri.transform.position;
        currentShadowTetri.transform.localScale = tetri.transform.localScale;

        foreach (var sr in currentShadowTetri.GetComponentsInChildren<SpriteRenderer>(true))
        {
            sr.maskInteraction = SpriteMaskInteraction.None;
        }

        tetri.OnDragEvent += UpdateShadowPosition;
        tetri.OnEndDragEvent += HandleTetriEndDrag;
    }

    private void HandleTetriEndDrag()
    {
        if (currentShadowTetri != null)
        {

            // 获取 Tetri 的当前位置
            Vector3 tetriWorldPosition = currentShadowTetri.transform.position;

            Operation.Tetri operationTetri = currentShadowTetri.GetComponent<Operation.Tetri>();
            Model.Tetri.Tetri modelTetri = operationTetri.ModelTetri;
            // 尝试将 Tetri 放置到 OperationTableModel
            if (operationTableController.TryPlaceTetri(modelTetri, tetriWorldPosition))
            {
                tetriInventoryController.MarkTetriAsUsed(modelTetri);
                Debug.Log("Tetri placed successfully!");

            }
            // 销毁影子物体
            Destroy(currentShadowTetri);
            currentShadowTetri = null;
        }
    }

    private void UpdateShadowPosition(Vector3 newPosition)
    {
        if (currentShadowTetri != null)
        {
            Vector3 tablePosition = operationTableController.transform.position;
            float cellSize = 1f;   // 每个格子的单位大小为 1
            Vector3 newVector;

            float snappedX = Mathf.Round((newPosition.x - tablePosition.x) / cellSize) * cellSize + tablePosition.x + cellSize / 2;
            float snappedY = Mathf.Round((newPosition.y - tablePosition.y) / cellSize) * cellSize + tablePosition.y - cellSize / 2;

            // 返回吸附后的坐标
            newVector = new Vector3(snappedX, snappedY, newPosition.z);

            newVector.x -= 1 * cellSize;
            newVector.y += 4 * cellSize;
            // 更新影子物体的位置
            currentShadowTetri.transform.position = newVector;
        }
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
