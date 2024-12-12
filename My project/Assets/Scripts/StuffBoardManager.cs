using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class StuffBoardManager : MonoBehaviour
{
    // [SerializeField] private Tilemap basketBoard;
    [SerializeField] private Tilemap stuffBoard;
    [SerializeField] private Tilemap maskBoard;
    [SerializeField] private TileBase tile;
    [SerializeField] private TetriBasketManager basketManager;
    [SerializeField] private GameObject stuffPrefab;

    private Dictionary<int, Stuff> stuffDictionary;
    private int nextStuffId;
    private Transform stuffsParent;

    void Start()
    {
        basketManager.Initialize();
        stuffDictionary = new Dictionary<int, Stuff>();
        nextStuffId = 0;

        // 创建一个名为 "Stuffs" 的子对象
        GameObject stuffsObject = new GameObject("Stuffs");
        stuffsObject.transform.SetParent(transform);
        stuffsParent = stuffsObject.transform;
    }

    public bool AddStuff(Tetri tetri)
    {
        
        TetriBasket freeBasket = basketManager.GetFreeBasket();
        if (freeBasket == null)
        {
            Debug.LogWarning("No free basket available.");
            return false;
        }

        int stuffId = nextStuffId++;

        // 初始化一个新的 stuffPrefab
        GameObject newStuff = Instantiate(stuffPrefab, stuffsParent);
        Stuff stuffComponent = newStuff.GetComponent<Stuff>();
        stuffComponent.Initialize(tetri, freeBasket.BasePosition, freeBasket);
        Tilemap newStuffTilemap = newStuff.GetComponent<Tilemap>();

        stuffDictionary[stuffId] = stuffComponent;

        foreach (var pos in tetri.RelativePositions)
        {
            Vector3Int tilePosition = freeBasket.BasePosition + pos;
            newStuffTilemap.SetTile(tilePosition, tile);
        }

        freeBasket.IsOccupied = true;
        return true;
    }

    public void RemoveStuff(Stuff stuff)
    {
        int stuffId = -1;
        foreach (var kvp in stuffDictionary)
        {
            if (kvp.Value == stuff)
            {
                stuffId = kvp.Key;
                break;
            }
        }

        if (stuffId != -1)
        {
            stuffDictionary.Remove(stuffId);
            stuff.OccupiedBasket.IsOccupied = false; // 将basket的isOccupied设置为false
            Destroy(stuff.gameObject);
        }
    }

    private void RefreshStuffBoard()
    {
        // 清除所有现有的 stuffPrefab
        foreach (Transform child in stuffsParent)
        {
            Destroy(child.gameObject);
        }

        // 重新绘制所有 Stuff 对象
        foreach (var kvp in stuffDictionary)
        {
            Stuff stuff = kvp.Value;
            Tetri tetri = stuff.Tetri;
            TetriBasket basket = basketManager.GetBasketById(kvp.Key);
            if (basket != null)
            {
                // 初始化一个新的 stuffPrefab
                GameObject newStuff = Instantiate(stuffPrefab, stuffsParent);
                Stuff stuffComponent = newStuff.GetComponent<Stuff>();
                stuffComponent.Initialize(tetri, basket.BasePosition, basket);

                Tilemap newStuffTilemap = newStuff.GetComponent<Tilemap>();

                foreach (var pos in tetri.RelativePositions)
                {
                    Vector3Int tilePosition = basket.BasePosition + pos;
                    newStuffTilemap.SetTile(tilePosition, tile);
                }
            }
        }
    }

}