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


        public event Action<Unit.Faction> OnFactionDefeated;


        [SerializeField] private Model.Inventory inventoryData;
        [SerializeField] private Model.Inventory enemyData;
        [SerializeField] private CharacterTypePrefabMapping characterTypePrefabMapping;
        private Coroutine spawnUnitsCoroutine;

        void Start()
        {
            // 初始化字典
            factionUnits[Unit.Faction.FactionA] = new List<Unit>();
            factionUnits[Unit.Faction.FactionB] = new List<Unit>();

        }

        public void SetEnemyData(List<Model.InventoryItem> enemyData)
        {
            this.enemyData.Items = enemyData; // 替换敌人数据
        }

        public void StartSpawningUnits()
        {
            spawnUnitsCoroutine = StartCoroutine(SpawnUnits());
        }

        private void StopSpawningUnits()
        {
            if (spawnUnitsCoroutine != null)
            {
                StopCoroutine(spawnUnitsCoroutine);
                spawnUnitsCoroutine = null;
            }
        }

        private IEnumerator SpawnUnits()
        {
            foreach (var inventoryItem in inventoryData.Items)
            {
                yield return StartCoroutine(SpawnUnitsA(inventoryItem));
            }

            // 启动阵营B的生成协程
            foreach (var enemyItem in enemyData.Items)
            {
                yield return StartCoroutine(SpawnUnitsB(enemyItem));
            }
        }

        IEnumerator SpawnUnitsA(Model.InventoryItem item)
        {   

            if (item.spawnInterval <= 0)
            {
                SpawnUnit(spawnPointA, characterTypePrefabMapping.GetPrefab(item.CharacterCell), Unit.Faction.FactionA, factionAParent, item.TetriCells);
                yield break;
            }
            while (true)
            {
                // 刷新阵营A的Unit
                SpawnUnit(spawnPointA, characterTypePrefabMapping.GetPrefab(item.CharacterCell), Unit.Faction.FactionA, factionAParent, item.TetriCells);

                // 等待一段时间后再次刷新
                yield return new WaitForSeconds(item.spawnInterval);
            }
        }

        IEnumerator SpawnUnitsB(Model.InventoryItem item)
        {
            if (item.spawnInterval <= 0)
            {
                SpawnUnit(spawnPointB, characterTypePrefabMapping.GetPrefab(item.CharacterCell), Unit.Faction.FactionB, factionBParent, item.TetriCells);
                yield break;
            }
            while (true)
            {
                // 刷新阵营B的Unit
                SpawnUnit(spawnPointB, characterTypePrefabMapping.GetPrefab(item.CharacterCell), Unit.Faction.FactionB, factionBParent, item.TetriCells);

                // 等待一段时间后再次刷新
                yield return new WaitForSeconds(item.spawnInterval);
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
        }

        private void ShowDamageText(Vector3 worldPosition, float damage)
        {
            if (damageTextPrefab != null && damageCanvas != null)
            {
                float randomOffsetX = UnityEngine.Random.Range(-0.2f, 0.2f); // 随机偏移范围
                Vector3 offsetPosition = worldPosition + new Vector3(randomOffsetX, 0f, 0f);

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
                    Debug.Log(deadUnit.faction + " 全部死亡");
                    StopSpawningUnits();

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
                    // todo , 根据不同阵营全部死亡触发不同的情况
                    OnFactionDefeated?.Invoke(deadUnit.faction);
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
        
    }
}
