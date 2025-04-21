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
        // [SerializeField] private CharacterTypePrefabMapping characterTypePrefabMapping;
        [SerializeField] private GameObject unitPrefab;
        [SerializeField] private TetriCellTypeResourceMapping tetriCellTypeResourceMapping;


        private Dictionary<Unit.Faction, List<Unit>> factionUnits = new();
        private List<Unit> allUnits = new();
        public event Action OnBattleEnd;

        private const float RandomOffsetRangeX = 1f;
        private const float RandomOffsetRangeY = 0.5f;


        void Start()
        {
            factionUnits[Unit.Faction.FactionA] = new List<Unit>();
            factionUnits[Unit.Faction.FactionB] = new List<Unit>();
            battleStatistics.OnEndStatistics += EndStatistics;
            
        }

        public void SetEnemyData(List<Model.InventoryItem> enemyData)
        {
            this.enemyData.Items = enemyData; // 替换敌人数据
        }

        public void StartNewLevelBattle(int level)
        {
            statisticsController.SetLevel(level); // 设置当前关卡
            SpawnUnits();
        }


        private void SpawnUnits()
        {
            SpawnFactionUnits(inventoryData.Items, spawnPointA, Unit.Faction.FactionA, factionAParent);
            SpawnFactionUnits(enemyData.Items, spawnPointB, Unit.Faction.FactionB, factionBParent);
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
                InitializeUnit(unitComponent, faction, characterCell, tetriCells, newUnit);
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

        private void InitializeUnit(Unit unit, Unit.Faction faction, Model.Tetri.Character characterCell, List<Model.Tetri.Cell> tetriCells, GameObject unitObject)
        {
            unit.SetFactionParent(factionAParent, factionBParent);
            unit.SetFaction(faction);
            unit.SetBattlefieldBounds(battlefieldMinBounds, battlefieldMaxBounds);
            var characterSprite = tetriCellTypeResourceMapping.GetSprite(characterCell);
            unit.BodySpriteRenderer.sprite = characterSprite;
            unit.Fist1SpriteRenderer.sprite = characterSprite;
            unit.Fist2SpriteRenderer.sprite = characterSprite;


            characterCell.Apply(unit);
            if (tetriCells != null)
            {
                foreach (Model.Tetri.Cell cell in tetriCells)
                {
                    cell.Apply(unit);
                    if (cell is Character featureCell)
                    {
                        unitObject.name = featureCell.CharacterName;
                    }
                }
            }

            unit.OnDeath += OnUnitDeath;
            unit.OnDamageTaken += HandleDamageTaken;
            factionUnits[faction].Add(unit);
            allUnits.Add(unit);
            unit.Initialize();
        }

        private void HandleDamageTaken(Units.Damages.Damage damage)
        {
            battleStatistics.AddRecord(damage);
            ShowDamageText(damage.TargetUnit.transform.position, damage.Value);
        }

        private void ShowDamageText(Vector3 worldPosition, float damage)
        {
            if (damageTextPrefab != null && damageCanvas != null)
            {
                float randomOffsetX = UnityEngine.Random.Range(-0.2f, 0.2f); // X轴随机偏移范围
                float randomOffsetY = UnityEngine.Random.Range(-0.1f, 0.1f); // Y轴随机偏移范围（X的一半）
                Vector3 offsetPosition = worldPosition + new Vector3(randomOffsetX, randomOffsetY, 0f);


                // 创建伤害文本实例
                GameObject damageTextInstance = Instantiate(damageTextPrefab, damageCanvas.transform);

                // 将世界坐标转换为屏幕坐标
                damageTextInstance.transform.position = offsetPosition;

                TextMeshProUGUI damageText = damageTextInstance.GetComponent<TextMeshProUGUI>();
                if (damageText != null)
                {
                    int roundedDamage = Mathf.RoundToInt(damage); // 将伤害值取整
                    damageText.text = roundedDamage.ToString();
                    StartCoroutine(FadeAndDestroyDamageText(damageTextInstance, damageText));
                }
            }
        }

        private IEnumerator FadeAndDestroyDamageText(GameObject damageTextInstance, TextMeshProUGUI damageText)
        {
            Color originalColor = damageText.color;
            float duration = 1f; // 显示时间为1秒
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float alpha = Mathf.Lerp(1f, 0f, elapsedTime / duration); // 渐变透明度
                damageText.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
                yield return null;
            }

            Destroy(damageTextInstance); // 销毁实例
        }

        private void OnUnitDeath(Unit deadUnit)
        {
            statisticsController.AddScore(1);// todo 以后根据不同单位设置不同分数
            // 从对应阵营里移除死亡单位
            if (factionUnits.ContainsKey(deadUnit.faction))
            {
                factionUnits[deadUnit.faction].Remove(deadUnit);
                // 如果某阵营单位列表为空，则表示该阵营全部阵亡
                if (factionUnits[deadUnit.faction].Count == 0)
                {
                    // 如果是 FactionA 全部死亡，更新生命值
                    if (deadUnit.faction == Unit.Faction.FactionA)
                    {
                        statisticsController.DecreaseLife(1); // 减少生命值
                        Debug.Log("FactionA 全部死亡，生命值减少 1");
                    }
                    
                    // 调用其他阵营所有幸存单位的 StopAction 方法
                    foreach (var faction in factionUnits.Keys)
                    {

                        foreach (var unit in factionUnits[faction])
                        {
                            unit.StopAction();
                        }
                        
                    }
                    battleStatistics.gameObject.SetActive(true);
                    battleStatistics.SetChoosenFaction(Units.Unit.Faction.FactionA);
                }
            }
        }

        private void DestroyAllUnits()
        {
            foreach (var unit in allUnits)
            {
                if (unit != null)
                {
                    Destroy(unit.gameObject);
                }
            }
            allUnits.Clear();

            foreach (var faction in factionUnits.Keys)
            {
                factionUnits[faction].Clear();
            }
        }


        public void EndStatistics()
        {
            battleStatistics.gameObject.SetActive(false);
            DestroyAllUnits(); // 销毁所有单位
            OnBattleEnd?.Invoke();
        }

        
    }
}
