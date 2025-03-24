using System;
using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEditor.SearchService;
using UnityEngine;
using Units;
using Model.Tetri;

namespace Controller {
    public class BattleField : MonoBehaviour
    {
        private Dictionary<Unit.Faction, List<Unit>> factionUnits 
            = new Dictionary<Unit.Faction, List<Unit>>();

        public Transform spawnPointA; // 阵营A的复活点
        public Transform spawnPointB; // 阵营B的复活点
        public Transform factionAParent; // 阵营A的父对象
        public Transform factionBParent; // 阵营B的父对象


        public event Action<Unit.Faction> OnFactionDefeated;


        [SerializeField] private Model.Inventory inventoryData;
        [SerializeField] private Model.Inventory enemyData;
        private Coroutine spawnUnitsCoroutine;

        // Start is called before the first frame update
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
                if (inventoryItem.IsEmpty)
                {
                    continue;
                }
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
                SpawnUnit(spawnPointA, item.Prefab, Unit.Faction.FactionA, factionAParent, item.TetriCells);
                yield break;
            }
            while (true)
            {
                // 刷新阵营A的Unit
                SpawnUnit(spawnPointA, item.Prefab, Unit.Faction.FactionA, factionAParent, item.TetriCells);

                // 等待一段时间后再次刷新
                yield return new WaitForSeconds(item.spawnInterval);
            }
        }

        IEnumerator SpawnUnitsB(Model.InventoryItem item)
        {
            if (item.spawnInterval <= 0)
            {
                SpawnUnit(spawnPointB, item.Prefab, Unit.Faction.FactionB, factionBParent, item.TetriCells);
                yield break;
            }
            while (true)
            {
                // 刷新阵营B的Unit
                SpawnUnit(spawnPointB, item.Prefab, Unit.Faction.FactionB, factionBParent, item.TetriCells);

                // 等待一段时间后再次刷新
                yield return new WaitForSeconds(item.spawnInterval);
            }
        }

        void SpawnUnit(Transform spawnPoint, GameObject unitPrefab, Unit.Faction faction, Transform parent, List<TetriCell> tetriCells)
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
                if (tetriCells != null) 
                {
                    foreach (var cell in tetriCells)
                    {
                        // 根据cell的类型或其他属性增加Unit的属性值
                        if (cell is TetriCellAttribute attributeCell)
                        {
                            attributeCell.ApplyAttributes(unitComponent);
                        }
                    }
                }
                
                unitComponent.SetFaction(faction);
                // 监听死亡事件
                unitComponent.OnDeath += OnUnitDeath;
                // 加入到列表
                factionUnits[faction].Add(unitComponent);
            }

        }

        private void OnUnitDeath(Unit deadUnit)
        {
            // 从对应阵营里移除死亡单位
            if (factionUnits.ContainsKey(deadUnit.unitFaction))
            {
                factionUnits[deadUnit.unitFaction].Remove(deadUnit);
                // 如果某阵营单位列表为空，则表示该阵营全部阵亡
                if (factionUnits[deadUnit.unitFaction].Count == 0)
                {
                    Debug.Log(deadUnit.unitFaction + " 全部死亡");
                    StopSpawningUnits();
                    // todo , 根据不同阵营全部死亡触发不同的情况
                    OnFactionDefeated?.Invoke(deadUnit.unitFaction);
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
