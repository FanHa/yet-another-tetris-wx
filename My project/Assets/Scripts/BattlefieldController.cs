using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;

public class BattlefieldController : MonoBehaviour
{
    public Transform spawnPointA; // 阵营A的复活点
    public Transform spawnPointB; // 阵营B的复活点
    public Transform factionAParent; // 阵营A的父对象
    public Transform factionBParent; // 阵营B的父对象
    
    public float spawnIntervalB = 5f; // 刷新时间间隔
    public Color colorFactionA = Color.red; // 阵营A的颜色
    public Color colorFactionB = Color.blue; // 阵营B的颜色


    [SerializeField] private Model.Inventory inventoryData;
    [SerializeField] private Model.Inventory enemyData;

    // Start is called before the first frame update
    void Start()
    {

        foreach (var inventoryItem in inventoryData.Items)
        {
            StartCoroutine(SpawnUnitsA(inventoryItem));
        }
        // 启动阵营B的生成协程
        foreach (var enemyItem in enemyData.Items)
        {
            StartCoroutine(SpawnUnitsB(enemyItem));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator SpawnUnitsA(Model.InventoryItem item)
    {   

        if (item.spawnInterval <= 0)
        {
            SpawnUnit(spawnPointA, item.Unit.Prefab, Unit.Faction.FactionA, colorFactionA, factionAParent);
            yield break;
        }
        while (true)
        {
            // 刷新阵营A的Unit
            SpawnUnit(spawnPointA, item.Unit.Prefab, Unit.Faction.FactionA, colorFactionA, factionAParent);

            // 等待一段时间后再次刷新
            yield return new WaitForSeconds(item.spawnInterval);
        }
    }

    IEnumerator SpawnUnitsB(Model.InventoryItem item)
    {
        if (item.spawnInterval <= 0)
        {
            SpawnUnit(spawnPointB, item.Unit.Prefab, Unit.Faction.FactionB, colorFactionB, factionBParent);
            yield break;
        }
        while (true)
        {
            // 刷新阵营B的Unit
            SpawnUnit(spawnPointB, item.Unit.Prefab, Unit.Faction.FactionB, colorFactionB, factionBParent);

            // 等待一段时间后再次刷新
            yield return new WaitForSeconds(item.spawnInterval);
        }
    }

    void SpawnUnit(Transform spawnPoint, GameObject unitPrefab, Unit.Faction faction, Color factionColor, Transform parent)
    {
        if (unitPrefab == null)
        {
            Debug.LogWarning("Unit prefab is null for faction: " + faction);
            return;
        }

        // 实例化Unit
        GameObject newUnit = Instantiate(unitPrefab, spawnPoint.position, spawnPoint.rotation, parent);
        Unit unitComponent = newUnit.GetComponent<Unit>();
        if (unitComponent != null)
        {
            unitComponent.unitFaction = faction;
        }

        // 设置SpriteRenderer的颜色
        SpriteRenderer spriteRenderer = newUnit.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.color = factionColor;
        }
    }

    void SpawnUnit(Transform spawnPoint, List<GameObject> unitPrefabs, Unit.Faction faction, Color factionColor, Transform parent)
    {
        if (unitPrefabs.Count == 0)
        {
            Debug.LogWarning("Unit prefabs list is empty for faction: " + faction);
            return;
        }

        // 随机选择一个Unit预制件
        int index = Random.Range(0, unitPrefabs.Count);
        GameObject unitPrefab = unitPrefabs[index];

        // 实例化Unit
        GameObject newUnit = Instantiate(unitPrefab, spawnPoint.position, spawnPoint.rotation, parent);
        Unit unitComponent = newUnit.GetComponent<Unit>();
        if (unitComponent != null)
        {
            unitComponent.unitFaction = faction;
        }

        // 设置SpriteRenderer的颜色
        SpriteRenderer spriteRenderer = newUnit.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.color = factionColor;
        }
    }

}