using System.Collections.Generic;
using UnityEngine;
using UI;
using Controller;
using Model.Tetri;
using Model;
using System.Collections;
using UnityEngine.UI;
using System;
using Units;
using UI.UnitInfo;
using UI.TetriInfo;

public class GameController : MonoBehaviour
{

    [SerializeField] private BattleField battleField;
    [SerializeField] private Controller.RewardController rewardController;

    [SerializeField] private Controller.UnitInventoryController unitInventoryController;
    [SerializeField] private Model.LevelConfig levelConfig; // 关卡配置
    [SerializeField] private Button battleButton;

    [SerializeField] private Controller.TetriInventoryController tetriInventoryController;
    [SerializeField] private Controller.OperationTableController operationTableController;
    [SerializeField] private UnitInfo unitInfo;
    [SerializeField] private TetriInfo tetriInfo;
    private GameObject currentShadowTetri;
    [SerializeField] private Operation.TetriFactory tetriFactory;
    private Operation.Tetri draggingTetriFromOperationTable;

    private void Start()
    {
        // 初始化资源面板和操作表
        battleField.OnBattleEnd += HandleBattleEnd;
        battleField.OnUnitClicked += HandleUnitClicked;
        levelConfig.Reset();
        // 绑定撤销操作按钮的点击事件
        // revokeOperationButton.onClick.AddListener(UndoLastPlacement);
        battleButton.onClick.AddListener(HandleBattleClicked);

        tetriInventoryController.OnTetriBeginDrag += HandleInventoryTetriBeginDrag;
        tetriInventoryController.OnTetriClick += HandleTetriClick;
        operationTableController.OnTetriBeginDrag += HandleOperationTableTetriBeginDrag;
        operationTableController.OnCharacterInfluenceGroupsChanged += HandleOperationTableGridCellUpdate;
        unitInventoryController.OnUnitClicked += HandleUnitClicked;
    }

    private void HandleUnitClicked(Unit unit)
    {
        unitInfo.ShowUnitInfo(unit);
    }

    private void HandleTetriClick(Operation.Tetri tetri)
    {
        tetriInfo.ShowTetriInfo(tetri.ModelTetri);
    }

    private void HandleOperationTableGridCellUpdate(List<CharacterInfluenceGroup> characterGroups)
    {
        unitInventoryController.RefreshInventoryFromInfluenceGroups(characterGroups);
        Debug.Log("OperationTable grid cells updated.");
    }

    private void HandleOperationTableTetriBeginDrag(Operation.Tetri tetri)
    {
        tetri.transform.SetParent(null); // 将其移到场景根节点或其他适当的父级
        draggingTetriFromOperationTable = tetri;
        draggingTetriFromOperationTable.name = "DraggingTetriFromOperationTable";
        foreach (var renderer in draggingTetriFromOperationTable.GetComponentsInChildren<Renderer>())
        {
            renderer.enabled = false;
        }
        operationTableController.RemoveTetri(tetri.ModelTetri);

        Operation.Tetri operationTetri = tetriFactory.CreateTetri(tetri.ModelTetri);
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
            Vector3 tetriWorldPosition = currentShadowTetri.transform.position;

            Operation.Tetri operationTetri = currentShadowTetri.GetComponent<Operation.Tetri>();
            Model.Tetri.Tetri modelTetri = operationTetri.ModelTetri;

            // 尝试将 Tetri 放置到 OperationTableModel
            if (operationTableController.TryPlaceTetri(modelTetri, tetriWorldPosition))
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

    private void HandleInventoryTetriBeginDrag(Operation.Tetri tetri)
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

            float snappedX = Mathf.Round((newPosition.x - tablePosition.x) / cellSize) * cellSize + tablePosition.x ;
            float snappedY = Mathf.Round((newPosition.y - tablePosition.y) / cellSize) * cellSize + tablePosition.y ;

            // 返回吸附后的坐标
            newVector = new Vector3(snappedX, snappedY, newPosition.z);
            newVector.y += 2 * cellSize;
            // 更新影子物体的位置
            currentShadowTetri.transform.position = newVector;
        }
    }

    private void HandleBattleEnd()
    {
        rewardController.EnterRewardSelectionPhase();
        rewardController.OnRewardSelected += HandleRewardSelected;
    }

    private void HandleRewardSelected()
    {
        rewardController.OnRewardSelected -= HandleRewardSelected;
        Camera.main.transform.position = new Vector3(0, 0, -10); // todo magic num
        levelConfig.AdvanceToNextLevel(); // 关卡增加

    }


    private void HandleBattleClicked()
    {
        List<UnitInventoryItem> levelData = levelConfig.GetEnemyData(); // 获取当前关卡数据
        unitInventoryController.SetEnemyInventoryData(levelData);
        Camera.main.transform.position = new Vector3(battleField.transform.position.x, battleField.transform.position.y, Camera.main.transform.position.z);
        battleField.StartNewLevelBattle(levelConfig.currentLevel);

    }

    private void OnDestroy()
    {
        // 取消监听UI的事件
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
        unitInventoryController.PrepareTrainGroundUnitInventory();
        Camera.main.transform.position = new Vector3(battleField.transform.position.x, battleField.transform.position.y, Camera.main.transform.position.z);
        battleField.StartTrainGround();
    }
#endif
}
