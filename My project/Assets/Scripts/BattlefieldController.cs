using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattlefieldController : MonoBehaviour
{
    public Transform spawnPointA; // 阵营A的复活点
    public Transform spawnPointB; // 阵营B的复活点
    public Transform factionAParent; // 阵营A的父对象
    public Transform factionBParent; // 阵营B的父对象
    public List<SpawnConfig> spawnConfigsB; // 阵营B的Unit生成配置列表
    public float spawnIntervalB = 5f; // 刷新时间间隔
    public Color colorFactionA = Color.red; // 阵营A的颜色
    public Color colorFactionB = Color.blue; // 阵营B的颜色
    private List<SpawnConfig> spawnConfigsA; // 阵营A的Unit生成配置列表

    // Start is called before the first frame update
    void Start()
    {
        spawnConfigsA = GameDataManager.Instance.spawnConfigsA;

        // 启动阵营A的生成协程
        foreach (var config in spawnConfigsA)
        {
            StartCoroutine(SpawnUnitsA(config));
        }

        // 启动阵营B的生成协程
        foreach (var config in spawnConfigsB)
        {
            StartCoroutine(SpawnUnitsB(config));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator SpawnUnitsA(SpawnConfig config)
    {
        if (config.spawnInterval <= 0)
        {
            SpawnUnit(spawnPointA, config.unitPrefab, Unit.Faction.FactionA, colorFactionA, factionAParent);
            yield break;
        }
        while (true)
        {
            // 刷新阵营A的Unit
            SpawnUnit(spawnPointA, config.unitPrefab, Unit.Faction.FactionA, colorFactionA, factionAParent);

            // 等待一段时间后再次刷新
            yield return new WaitForSeconds(config.spawnInterval);
        }
    }

    IEnumerator SpawnUnitsB(SpawnConfig config)
    {
        while (true)
        {
            // 刷新阵营B的Unit
            SpawnUnit(spawnPointB, config.unitPrefab, Unit.Faction.FactionB, colorFactionB, factionBParent);

            // 等待一段时间后再次刷新
            yield return new WaitForSeconds(config.spawnInterval);
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