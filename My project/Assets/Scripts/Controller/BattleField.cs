using System;
using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEditor.SearchService;
using UnityEngine;
using Units;
using Model.Tetri;
using System.Linq;
using TMPro;

namespace Controller {
    public class BattleField : MonoBehaviour
    {
        [SerializeField] private GameObject damageTextPrefab;
        [SerializeField] private Canvas damageCanvas;
        [SerializeField] private Controller.Statistics statisticsController; // 统计控制器
        private Dictionary<Unit.Faction, List<Unit>> factionUnits 
            = new Dictionary<Unit.Faction, List<Unit>>();

        public Transform spawnPointA; // 阵营A的复活点
        public Transform spawnPointB; // 阵营B的复活点
        public Transform factionAParent; // 阵营A的父对象
        public Transform factionBParent; // 阵营B的父对象

        public event Action OnFactionDefeated;

        [SerializeField] private Model.Inventory inventoryData;
        [SerializeField] private Model.Inventory enemyData;
        [SerializeField] private CharacterTypePrefabMapping characterTypePrefabMapping;
        private Dictionary<string, Units.Statistics> unitStatistics = new Dictionary<string, Units.Statistics>(); // 使用单位名称作为键
        [SerializeField] private GameObject statisticsPanel; // 统计面板
        [SerializeField] private GameObject unitStatisticPrefab; // 单位统计预制体
        [SerializeField] private GameObject damageTypeStatisticPrefab; // 伤害类型统计预制体
        void Start()
        {
            // 初始化字典
            factionUnits[Unit.Faction.FactionA] = new List<Unit>();
            factionUnits[Unit.Faction.FactionB] = new List<Unit>();
        }

        public void SetEnemyData(List<Model.InventoryItem> enemyData)
        {
            // if (this.enemyData.Items == null)
            // {
            //     this.enemyData.Items = new List<Model.InventoryItem>();
            // }
            this.enemyData.Items = enemyData; // 替换敌人数据
        }

        public void StartSpawningUnits()
        {
            SpawnUnits();
        }

        private  void SpawnUnits()
        {
            foreach (var inventoryItem in inventoryData.Items)
            {
                SpawnUnit(spawnPointA, characterTypePrefabMapping.GetPrefab(inventoryItem.CharacterCell), Unit.Faction.FactionA, factionAParent, inventoryItem.TetriCells);

            }

            // 启动阵营B的生成协程
            foreach (var enemyItem in enemyData.Items)
            {
                SpawnUnit(spawnPointB, characterTypePrefabMapping.GetPrefab(enemyItem.CharacterCell), Unit.Faction.FactionB, factionBParent, enemyItem.TetriCells);

            }
        }



        void SpawnUnit(Transform spawnPoint, GameObject unitPrefab, Unit.Faction faction, Transform parent, List<Cell> tetriCells)
        {
            if (unitPrefab == null)
            {
                Debug.LogWarning("Unit prefab is null for faction: " + faction);
                return;
            }

            // 生成随机偏移量
            Vector3 randomOffset = new Vector3(
                UnityEngine.Random.Range(-1f, 1f), // X轴随机偏移
                UnityEngine.Random.Range(-1f, 1f), // Y轴随机偏移
                0f // Z轴保持不变
            );
            Vector3 spawnPosition = spawnPoint.position + randomOffset;
            // 实例化Unit
            
            GameObject newUnit = Instantiate(unitPrefab, spawnPosition, spawnPoint.rotation, parent);
            Unit unitComponent = newUnit.GetComponent<Unit>();
            if (unitComponent != null)
            {
                unitComponent.SetFactionParent(factionAParent, factionBParent);
                if (tetriCells != null) 
                {
                    // 先处理 Attribute 类型
                    foreach (Cell cell in tetriCells)
                    {
                        if (cell is IBaseAttribute attributeCell)
                        {
                            attributeCell.ApplyAttributes(unitComponent);
                        }
                        if (cell is ICharacterFeature featureCell)
                        {
                            featureCell.ApplyCharacterFeature(unitComponent);
                        }
                    }
                }
                
                unitComponent.SetFaction(faction);
                // 监听死亡事件
                unitComponent.OnDeath += OnUnitDeath;
                unitComponent.OnDamageTaken += HandleDamageTaken;
                // 加入到列表
                factionUnits[faction].Add(unitComponent);
                unitComponent.Initialized();
            }

        }

        private void HandleDamageTaken(Units.Damages.EventArgs args)
        {
            ShowDamageText(args.Target.transform.position, args.Damage.Value);
            if (args.Source == null || args.Source.faction != Unit.Faction.FactionA)
                return;
            string sourceName = args.Source.name;
            if (!unitStatistics.ContainsKey(sourceName))
            {
                unitStatistics[sourceName] = new Units.Statistics(sourceName);
            }
            unitStatistics[sourceName].AddDamage(args.Damage.DamageType, args.Damage.Value);
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
                    damageText.text = damage.ToString();
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
                        if (faction != deadUnit.faction) // 排除已死亡的阵营
                        {
                            foreach (var unit in factionUnits[faction])
                            {
                                unit.StopAction();
                            }
                        }
                    }
                    ShowBattleStatistics();
                }
            }
        }

        public void DestroyAllUnits()
        {
            foreach (var faction in factionUnits.Keys)
            {
                foreach (var unit in factionUnits[faction])
                {
                    Destroy(unit.gameObject);
                }
                factionUnits[faction].Clear();
            }
        }

        public void ShowBattleStatistics()
        {
            statisticsPanel.SetActive(true);
            Transform contentTransform = statisticsPanel.transform.Find("Content");
            // 清空统计面板
            foreach (Transform child in contentTransform)
            {
                Destroy(child.gameObject);
            }

            var sortedUnitStatistics = unitStatistics.Values
                .OrderByDescending(stat => stat.DamageByType.Values.Sum())
                .ToList();

            // 遍历每个单位的统计数据
            foreach (var stat in sortedUnitStatistics)
            {
                // 创建 UnitStatisticPrefab
                GameObject unitStatInstance = Instantiate(unitStatisticPrefab, contentTransform.transform);
                var leftSide = unitStatInstance.transform.Find("LeftSide");
                if (leftSide == null)
                {
                    Debug.LogError("LeftSide object not found in UnitStatisticPrefab!");
                    continue;
                }

                // 获取 NameText 和 TotalDamageText
                var unitNameText = leftSide.Find("NameText").GetComponent<TextMeshProUGUI>();
                var totalDamageText = leftSide.Find("TotalDamageText").GetComponent<TextMeshProUGUI>();
                var damageTypePanel = unitStatInstance.transform.Find("DamageTypePanel");


                // 设置单位名称
                unitNameText.text = stat.UnitName;

                // 计算总伤害
                float totalDamage = stat.DamageByType.Values.Sum();
                totalDamageText.text = "总伤害:" + Mathf.RoundToInt(totalDamage).ToString(); // 转换为整数并设置文本


                // 按伤害值排序伤害类型
                var sortedDamageTypes = stat.DamageByType
                    .OrderByDescending(damageType => damageType.Value)
                    .ToList();
                // 遍历单位的伤害类型统计
                foreach (var damageType in sortedDamageTypes)
                {
                    // 创建 DamageTypeStatisticPrefab
                    GameObject damageTypeStatInstance = Instantiate(damageTypeStatisticPrefab, damageTypePanel);
                    var damageTypeText = damageTypeStatInstance.transform.Find("TypeText").GetComponent<TextMeshProUGUI>();
                    var damageValueText = damageTypeStatInstance.transform.Find("ValueText").GetComponent<TextMeshProUGUI>();

                    // 设置伤害类型和伤害值
                    damageTypeText.text = damageType.Key;
                    damageValueText.text = Mathf.RoundToInt(damageType.Value).ToString(); // 转换为整数
                }
            }
        }

        public void EndStatistics()
        {
            statisticsPanel.SetActive(false);
            OnFactionDefeated?.Invoke();
        }

        
    }
}
