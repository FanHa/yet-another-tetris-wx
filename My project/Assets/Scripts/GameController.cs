using System.Collections.Generic;
using UnityEngine;
using UI;
using Controller;
using Model.Tetri;
using Model;
using Model.UnitInventory;
using System.Collections;
using UnityEngine.UI;
using System;
using Units;
using UI.UnitInfo;
using UI.TetriInfo;

public class GameController : MonoBehaviour
{
    private enum UiPageState
    {
        Assembly,
        Preview,
        BattleRunning,
        BattlePaused,
        Reward
    }

    // 页面状态：先用于统一状态定义，后续步骤再接入显隐切换。
    private UiPageState currentUiPageState = UiPageState.Assembly;


    [SerializeField] private BattleField battleField;
    [SerializeField] private BattleProgressController battleProgressController;
    [SerializeField] private BattleSettlementController battleSettlementController;
    [SerializeField] private Controller.RewardController rewardController;

    private UnitInventoryModel playerUnitInventoryData;
    private UnitInventoryModel enemyUnitInventoryData;
    private UnitInventoryModel trainGroundFactionAInventoryData;
    private UnitInventoryModel trainGroundFactionBInventoryData;
    [SerializeField] private UnitInventoryInitConfig trainGroundFactionAInitConfig;
    [SerializeField] private UnitInventoryInitConfig trainGroundFactionBInitConfig;
    [SerializeField] private UnitInventoryFactory unitInventoryFactory;
    [SerializeField] private EnemySquadFactory enemySquadFactory;
    [SerializeField] private LevelState levelState;
    [SerializeField] private Button battleButton;
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button previewButton;
    [Header("UI Pages")]
    [SerializeField] private GameObject assemblyPage;
    [SerializeField] private GameObject battlePage;
    
    private bool isPaused = false;
    [SerializeField] private Controller.TetriInventoryController tetriInventoryController;
    [SerializeField] private Controller.OperationTableController operationTableController;
    [SerializeField] private UnitInfo unitInfo;
    [SerializeField] private TetriInfo tetriInfo;
    [SerializeField] private Units.UnitFactory unitFactory;
    [SerializeField] private UI.BattlePreview battlePreviewUI;
    private GameObject currentShadowTetri;
    [SerializeField] private View.Tetri.TetriViewFactory tetriFactory;
    private View.Tetri.TetriView draggingTetriFromOperationTable;
    private Units.Unit tempUnit;

    private void Awake()
    {
        playerUnitInventoryData = new UnitInventoryModel();
        enemyUnitInventoryData = new UnitInventoryModel();
        trainGroundFactionAInventoryData = new UnitInventoryModel();
        trainGroundFactionBInventoryData = new UnitInventoryModel();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (battleProgressController == null)
            Debug.LogError($"{nameof(battleProgressController)} is not assigned.", this);
        if (battleSettlementController == null)
            Debug.LogError($"{nameof(battleSettlementController)} is not assigned.", this);
        if (levelState == null)
        {
            Debug.LogError($"[{nameof(GameController)}] Level state is missing.", this);
        }

        if (enemySquadFactory == null)
        {
            Debug.LogError($"[{nameof(GameController)}] Enemy squad factory is missing.", this);
        }
        if (operationTableController == null)
        {
            Debug.LogError($"[{nameof(GameController)}] Operation table controller is missing.", this);
        }
    }
#endif

    private void ApplyUiPageState(UiPageState newState)
    {
        currentUiPageState = newState;

        bool showAssemblyPage = newState == UiPageState.Assembly;
        bool showBattlePage = newState == UiPageState.BattleRunning || newState == UiPageState.BattlePaused;

        assemblyPage.SetActive(showAssemblyPage);
        battlePage.SetActive(showBattlePage);
        
    }

    private void Start()
    {
        // 初始化资源面板和操作表
        battleSettlementController.OnSettlementConfirmed += HandleSettlementConfirmed;
        battleField.OnUnitClicked += HandleUnitClicked;
        levelState.Reset();

        battleButton.onClick.AddListener(HandleBattleClicked);
        pauseButton.onClick.AddListener(HandlePauseClicked);
        previewButton.onClick.AddListener(HandlePreviewButtonClicked);

        tetriInventoryController.OnTetriBeginDrag += HandleInventoryTetriBeginDrag;
        tetriInventoryController.OnTetriClick += HandleTetriInventoryTetriClick;
        operationTableController.OnTetriBeginDrag += HandleOperationTableTetriBeginDrag;
        operationTableController.OnTetriClick += HandleOperationTableTetriClick; // 新增：监听操作台点击
        battlePreviewUI.OnClosed += HandleBattlePreviewClosed;
        unitInfo.OnClosed += () =>
        {
            if (tempUnit != null)
            {
                Destroy(tempUnit.gameObject);
                tempUnit = null;
            }
        };

        ApplyUiPageState(UiPageState.Assembly);
    }

    

    private void HandleUnitClicked(Units.Unit unit)
    {
        unitInfo.ShowUnitInfo(unit);
    }

    private void HandleTetriInventoryTetriClick(View.Tetri.TetriView tetri)
    {
        tetriInfo.ShowTetriInfo(tetri);
    }

    private void HandleOperationTableTetriClick(View.Tetri.TetriView tetri)
    {
        switch (tetri)
        {
            case View.Tetri.TetriCharacterView characterView:
                UnitSpawnPlacement unitSpawnPlacement = operationTableController.GetUnitSpawnPlacementForTetri(characterView.Model);
                Units.Unit unit = unitFactory.CreatePreviewUnit(unitSpawnPlacement.CharacterBuild);
                unit.transform.position = characterView.transform.position;
                unitInfo.ShowUnitInfo(unit);
                tempUnit = unit;
                break;
            case View.Tetri.CellBasedTetriView:
                tetriInfo.ShowTetriInfo(tetri);
                break;
            default:
                throw new ArgumentException("Unsupported Tetri view type.", nameof(tetri));
        }
    }

    private void HandleOperationTableTetriBeginDrag(View.Tetri.TetriView tetri)
    {
        tetri.transform.SetParent(null); // 将其移到场景根节点或其他适当的父级
        draggingTetriFromOperationTable = tetri;
        draggingTetriFromOperationTable.name = "DraggingTetriFromOperationTable";
        foreach (var renderer in draggingTetriFromOperationTable.GetComponentsInChildren<Renderer>())
        {
            renderer.enabled = false;
        }
        operationTableController.RemoveTetri(tetri.ModelTetri);

        View.Tetri.TetriView operationTetri = tetriFactory.CreateTetri(tetri.ModelTetri);
        currentShadowTetri = operationTetri.gameObject;
        currentShadowTetri.name = "OperationTableDraggingTetriShadow";
        currentShadowTetri.transform.position = tetri.transform.position;
        currentShadowTetri.transform.localScale = tetri.transform.localScale;

        // 监听拖动事件
        tetri.OnDragEvent += UpdateShadowPosition;
        tetri.OnEndDragEvent += HandleOperationTableTetriDragEndEvent;
    }

    private void HandleOperationTableTetriDragEndEvent()
    {
        if (currentShadowTetri != null)
        {
            // 获取影子 Tetri 的当前位置
            Vector3 tetriCenterWorldPosition = currentShadowTetri.transform.position;

            View.Tetri.TetriView operationTetri = currentShadowTetri.GetComponent<View.Tetri.TetriView>();
            Model.Tetri.Tetri modelTetri = operationTetri.ModelTetri;

            // 尝试将 Tetri 放置到 OperationTableModel
            if (operationTableController.TryPlaceTetri(modelTetri, tetriCenterWorldPosition))
            {
                Debug.Log("Tetri replaced successfully!");
            }
            else
            {
                tetriInventoryController.AddTetri(modelTetri);
                Debug.Log("Failed to place Tetri.");
            }

            // 销毁影子物体
            Destroy(currentShadowTetri);
            currentShadowTetri = null;
            Destroy(draggingTetriFromOperationTable.gameObject);
            draggingTetriFromOperationTable = null;
        }
    }

    private void HandleInventoryTetriBeginDrag(View.Tetri.TetriView tetri)
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
        tetri.OnEndDragEvent += HandleInventoryTetriEndDrag;
    }

    private void HandleInventoryTetriEndDrag()
    {
        if (currentShadowTetri != null)
        {

            // 获取 Tetri 的当前位置
            Vector3 tetriCenterWorldPosition = currentShadowTetri.transform.position;

            View.Tetri.TetriView operationTetri = currentShadowTetri.GetComponent<View.Tetri.TetriView>();
            Model.Tetri.Tetri modelTetri = operationTetri.ModelTetri;
            // 尝试将 Tetri 放置到 OperationTableModel
            if (operationTableController.TryPlaceTetri(modelTetri, tetriCenterWorldPosition))
            {
                tetriInventoryController.MarkTetriAsUsed(modelTetri);
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
            currentShadowTetri.transform.position = operationTableController.SnapTetriCenterToGrid(newPosition);
        }
    }

    private void HandleSettlementConfirmed()
    {
        ApplyUiPageState(UiPageState.Reward);
        rewardController.EnterRewardSelectionPhase();
        rewardController.OnRewardSelected += HandleRewardSelected;
    }

    private void HandleRewardSelected()
    {
        rewardController.OnRewardSelected -= HandleRewardSelected;
        Camera.main.transform.position = new Vector3(0, 0, -10); // todo magic num
        levelState.AdvanceToNextLevel(); // 关卡增加
        ApplyUiPageState(UiPageState.Assembly);

    }

    private void UpdateBattleInventoryModels()
    {
        enemyUnitInventoryData.ResetInventoryData(BuildEnemyInventoryForCurrentLevel());
        playerUnitInventoryData.ResetInventoryData(operationTableController.GetUnitSpawnPlacements());
    }

    private void UpdateTrainGroundInventoryModels()
    {
        trainGroundFactionAInventoryData.ResetInventoryData(BuildTrainGroundInventory(trainGroundFactionAInitConfig));
        trainGroundFactionBInventoryData.ResetInventoryData(BuildTrainGroundInventory(trainGroundFactionBInitConfig));
    }

    private List<UnitSpawnPlacement> BuildEnemyInventoryForCurrentLevel()
    {
        var enemySquads = enemySquadFactory.Build(levelState);
        return unitInventoryFactory.Build(
            UnitInventoryBuildDataAdapter.FromEnemySquads(enemySquads));
    }

    private List<UnitSpawnPlacement> BuildTrainGroundInventory(UnitInventoryInitConfig initConfig)
    {
        return unitInventoryFactory.Build(
            UnitInventoryBuildDataAdapter.FromInitConfig(initConfig));
    }

    private void HandleBattleClicked()
    {
        UpdateBattleInventoryModels();
        Camera.main.transform.position = new Vector3(battleField.transform.position.x, battleField.transform.position.y, Camera.main.transform.position.z);
        battleField.PrepareBattle(
            playerUnitInventoryData.Items,
            enemyUnitInventoryData.Items);

        battleProgressController.SetLevel(levelState.CurrentLevel);
        battleProgressController.SetProgressEnabled(true);
        battleField.StartBattle();

        Time.timeScale = 1;
        isPaused = false;
        pauseButton.GetComponentInChildren<TMPro.TMP_Text>().text = "暂停";
        ApplyUiPageState(UiPageState.BattleRunning);

    }

    private void HandlePauseClicked()
    {
        if (isPaused)
        {
            Time.timeScale = 1;
            pauseButton.GetComponentInChildren<TMPro.TMP_Text>().text = "暂停";
            isPaused = false;
            ApplyUiPageState(UiPageState.BattleRunning);
        }
        else
        {
            Time.timeScale = 0;
            pauseButton.GetComponentInChildren<TMPro.TMP_Text>().text = "恢复";
            isPaused = true;
            ApplyUiPageState(UiPageState.BattlePaused);
        }
    }

    private void HandlePreviewButtonClicked()
    {
        UpdateBattleInventoryModels();
        var bfPos = battleField.transform.position;
        Camera.main.transform.position = new Vector3(bfPos.x, bfPos.y, Camera.main.transform.position.z);
        ApplyUiPageState(UiPageState.Preview);
        battlePreviewUI.Activate();
        battleField.PrepareBattle(
            playerUnitInventoryData.Items,
            enemyUnitInventoryData.Items);
    }

    private void HandleBattlePreviewClosed()
    {
        var mainPos = transform.position;
        Camera.main.transform.position = new Vector3(mainPos.x, mainPos.y, Camera.main.transform.position.z);
        battlePreviewUI.Deactivate();
        battleField.ClearField();
        ApplyUiPageState(UiPageState.Assembly);
    }

    
#if UNITY_EDITOR
    [ContextMenu("Test Enter Train Ground")]
    private void TestEnterTrainGround()
    {
        if (!Application.isPlaying)
        {
            Debug.LogWarning("This method can only be called in Play mode.");
            return;
        }
        UpdateTrainGroundInventoryModels();
        Camera.main.transform.position = new Vector3(battleField.transform.position.x, battleField.transform.position.y, Camera.main.transform.position.z);
        battleField.PrepareBattle(
            trainGroundFactionAInventoryData.Items,
            trainGroundFactionBInventoryData.Items);

        battleProgressController.SetProgressEnabled(false);
        battleField.StartBattle();
    }
#endif
}
