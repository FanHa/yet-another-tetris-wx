using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Units;
using Model.Tetri;
using System.Linq;
using TMPro;
using UnityEngine.UI;
using Model;
using Units.Damages;

namespace Controller {
    public class BattleField : MonoBehaviour
    {
        [Header("Battlefield Bounds")]
        [SerializeField] private Transform battlefieldMinBounds;
        [SerializeField] private Transform battlefieldMaxBounds;

        [Header("UI Elements")]
        [SerializeField] private GameObject damageTextPrefab;
        [SerializeField] private Canvas damageCanvas;
        [SerializeField] private BattleStatistics battleStatistics;

        [Header("Controllers")]
        [SerializeField] private Controller.Statistics statisticsController;

        [Header("Spawn Points")]
        public Transform spawnPointA;
        public Transform spawnPointB;
        public Transform factionAParent;
        public Transform factionBParent;

        [Header("Data")]
        [SerializeField] private Model.Inventory inventoryData;
        [SerializeField] private Model.Inventory enemyData;
        [SerializeField] private Model.TrainGround.Setup trainGroundSetup;
        [SerializeField] private GameObject unitPrefab;
        [SerializeField] private TetriCellTypeResourceMapping tetriCellTypeResourceMapping;

        [SerializeField] private UnitManager unitManager;
        public event Action OnBattleEnd;

        private const float RandomOffsetRangeX = 1f;
        private const float RandomOffsetRangeY = 0.5f;

        private List<InventoryItem> factionAConfig;
        private List<InventoryItem> factionBConfig;

        void Start()
        {
            battleStatistics.OnEndStatistics += EndStatistics;
        }

        public void SetEnemyData(List<Model.InventoryItem> enemyData)
        {
            this.enemyData.Items = enemyData; // 替换敌人数据
        }


        public void StartNewLevelBattle(int level)
        {
            statisticsController.SetLevel(level); // 设置当前关卡
            factionAConfig = inventoryData.Items;
            
            factionBConfig = enemyData.Items;
            SpawnUnits();
        }

        internal void StartTrainGroundBattle()
        {
            // 将 FactionAUnits 转换为 InventoryItem 列表
            factionAConfig = trainGroundSetup.FactionAUnits
                .Select(unitConfig => unitConfig.ToInventoryItem())
                .ToList();

            // 将 FactionBUnits 转换为 InventoryItem 列表
            factionBConfig = trainGroundSetup.FactionBUnits
                .Select(unitConfig => unitConfig.ToInventoryItem())
                .ToList();
            SpawnUnits();
        }


        private void SpawnUnits()
        {
            SpawnFactionUnits(factionAConfig, spawnPointA, Unit.Faction.FactionA, factionAParent);
            SpawnFactionUnits(factionBConfig, spawnPointB, Unit.Faction.FactionB, factionBParent);
        }

        private void SpawnFactionUnits(List<Model.InventoryItem> items, Transform spawnPoint, Unit.Faction faction, Transform parent)
        {
            foreach (Model.InventoryItem item in items)
            {
                SpawnUnit(spawnPoint, item.CharacterCell, faction, parent, item.TetriCells);
            }
        }

        private void SpawnUnit(Transform spawnPoint, Character characterCell, Unit.Faction faction, Transform parent, List<Model.Tetri.Cell> tetriCells)
        {

            Vector3 spawnPosition = GetRandomSpawnPosition(spawnPoint.position);
            GameObject newUnit = Instantiate(unitPrefab, spawnPosition, spawnPoint.rotation, parent);
            Unit unitComponent = newUnit.GetComponent<Unit>();

            if (unitComponent != null)
            {
                InitializeUnit(unitComponent, faction, characterCell, tetriCells);
                
            }

        }

        private Vector3 GetRandomSpawnPosition(Vector3 basePosition)
        {
            return basePosition + new Vector3(
                UnityEngine.Random.Range(-RandomOffsetRangeX, RandomOffsetRangeX),
                UnityEngine.Random.Range(-RandomOffsetRangeY, RandomOffsetRangeY),
                0f
            );
        }

        private void InitializeUnit(Unit unit, Unit.Faction faction, Model.Tetri.Character characterCell, List<Model.Tetri.Cell> tetriCells)
        {
            unit.unitManager = unitManager;
            unit.SetFaction(faction);
            unit.SetBattlefieldBounds(battlefieldMinBounds, battlefieldMaxBounds);
            var characterSprite = tetriCellTypeResourceMapping.GetSprite(characterCell);
            unit.BodySpriteRenderer.sprite = characterSprite;
            unit.Fist1SpriteRenderer.sprite = characterSprite;
            unit.Fist2SpriteRenderer.sprite = characterSprite;

            characterCell.Apply(unit);

            Model.Tetri.GarbageReuse garbageReuseCell = null; // 用于存储 GarbageReuse 类型的 Cell

            if (tetriCells != null)
            {
                foreach (Model.Tetri.Cell cell in tetriCells)
                {
                    cell.Apply(unit);
                    if (cell is Model.Tetri.GarbageReuse garbageReuse)
                    {
                        garbageReuseCell = garbageReuse; // 暂存 GarbageReuse 类型的 Cell
                    }
                }
            }
            garbageReuseCell?.Reuse(tetriCells, unit); // 调用 GarbageReuse 的方法，并传入 tetriCells


            unit.OnDeath += OnUnitDeath;
            unit.OnDamageTaken += HandleDamageTaken;
            unitManager.Register(unit);
            unit.Initialize();
        }

        private void HandleDamageTaken(Units.Damages.Damage damage)
        {
            battleStatistics.AddRecord(damage);
            ShowDamageText(damage.TargetUnit.transform.position, damage);
        }

        private void ShowDamageText(Vector3 worldPosition, Units.Damages.Damage damage)
        {
            if (damageTextPrefab != null && damageCanvas != null)
            {
                GameObject damageTextInstance = Instantiate(damageTextPrefab, damageCanvas.transform);
                DamageView damageview = damageTextInstance.GetComponent<DamageView>();
                string sourceName = "";
                if (damage.Type != DamageType.Hit)
                {
                    sourceName = damage.SourceName;
                }

                damageview.Initialize(damage.Value, sourceName, worldPosition);
            }
        }

        private void OnUnitDeath(Unit deadUnit)
        {
            statisticsController.AddScore(1);// todo 以后根据不同单位设置不同分数
            unitManager.Unregister(deadUnit);
            List<Unit> survivals = deadUnit.faction == Unit.Faction.FactionA 
                ? unitManager.GetFactionAUnits() : unitManager.GetFactionBUnits();
            if (survivals.Count == 0)
            {
                if (deadUnit.faction == Unit.Faction.FactionA)
                {
                    statisticsController.DecreaseLife(1);
                    Debug.Log("FactionA 全部死亡，生命值减少 1");
                }
                foreach (Unit unit in unitManager.GetAllUnits())
                {
                    unit.StopAction();
                }
                StartCoroutine(ShowBattleStatisticsWithDelay(2f));
            }
        }

        private IEnumerator ShowBattleStatisticsWithDelay(float delay)
        {
            yield return new WaitForSeconds(delay);

            battleStatistics.gameObject.SetActive(true);
            battleStatistics.ShowChoosenFaction(Units.Unit.Faction.FactionA);
        }

        private void DestroyAllUnits()
        {
            // 遍历 factionAParent 的所有子对象并销毁
            foreach (Transform child in factionAParent)
            {
                Destroy(child.gameObject);
            }

            // 遍历 factionBParent 的所有子对象并销毁
            foreach (Transform child in factionBParent)
            {
                Destroy(child.gameObject);
            }

            // 重置 UnitManager
            unitManager.Reset();
        }


        public void EndStatistics()
        {
            battleStatistics.gameObject.SetActive(false);
            DestroyAllUnits(); // 销毁所有单位
            OnBattleEnd?.Invoke();
        }

        
    }
}
