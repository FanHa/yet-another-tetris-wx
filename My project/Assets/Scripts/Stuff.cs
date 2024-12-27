#if !DISABLE_OBSOLETE_CLASSES
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stuff : MonoBehaviour
{
    public Tetri Tetri { get; private set; }
    public Vector3Int StuffBoardPosition { get; private set; }
    public TetriBasket OccupiedBasket { get; private set; }

    // 初始化方法
    public void Initialize(Tetri tetri, Vector3Int stuffBoardPosition, TetriBasket occupiedBasket)
    {
        Tetri = tetri;
        StuffBoardPosition = stuffBoardPosition;
        OccupiedBasket = occupiedBasket;
    }

    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
#endif