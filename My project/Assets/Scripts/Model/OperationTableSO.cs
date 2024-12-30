using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Model
{
    [CreateAssetMenu(fileName = "OperationTableSO", menuName = "ScriptableObjects/OperationTableSO", order = 1)]
    public class OperationTableSO : ScriptableObject
    {
        public event Action OnTableChanged;
        public event Action<RowClearedInfo> OnRowCleared; // 定义事件，参数为RowClearedInfo

        [SerializeField] private Brick emptyBrick; 
        [SerializeField] private Brick setedBrick;

        private Brick[,] board; // 棋盘
        private List<OperationTableTetri> tetriList; // 记录当前棋盘中的Tetri列表
        private int[,] tetriBoard; // 记录每个格子属于哪个Tetri
        private Dictionary<int, TetriInfo> tetriInfoDict; // 记录每个Tetri的信息


        public void Init(int rows, int columns)
        {
            board = new Brick[rows, columns];
            tetriList = new List<OperationTableTetri>();
            tetriBoard = new int[rows, columns];
            tetriInfoDict = new Dictionary<int, TetriInfo>();

             // 初始化棋盘，将所有格子变为type为"Setable"的Brick
            for (int x = 0; x < rows; x++)
            {
                for (int y = 0; y < columns; y++)
                {
                    board[x, y] = Instantiate(emptyBrick); // 使用一个示例Brick对象来填充棋盘
                }
            }
        }


        public void CheckAndClearFullRows()
        {
            for (int y = 0; y < board.GetLength(1); y++)
            {
                bool isFullRow = true;
                for (int x = 0; x < board.GetLength(0); x++)
                {
                    if (board[x, y].Tile == emptyBrick.Tile)
                    {
                        isFullRow = false;
                        break;
                    }
                }

                if (isFullRow)
                {
                    ClearRow(y);
                }
            }
        }

        private void ClearRow(int row)
        {
            List<Brick> clearedBricks = new List<Brick>();
            for (int x = 0; x < board.GetLength(0); x++)
            {
                clearedBricks.Add(board[x, row]);
                board[x, row] = new Brick(); // 清空该行的砖块
                tetriBoard[x, row] = 0; // 更新tetriBoard
            }

            // 更新TetriInfo todo 这里有点重,是否真的有必要保存这个信息
            foreach (var kvp in tetriInfoDict)
            {
                TetriInfo tetriInfo = kvp.Value;
                bool tetriExists = false;

                for (int i = 0; i < tetriInfo.tetri.shape.GetLength(0); i++)
                {
                    for (int j = 0; j < tetriInfo.tetri.shape.GetLength(1); j++)
                    {
                        int x = tetriInfo.startPoint.x + i;
                        int y = tetriInfo.startPoint.y + j;

                        if (x >= 0 && x < board.GetLength(0) && y >= 0 && y < board.GetLength(1) && board[x, y].Tile != emptyBrick.Tile)
                        {
                            tetriExists = true;
                            break;
                        }
                    }

                    if (tetriExists)
                    {
                        break;
                    }
                }

                tetriInfo.isActive = tetriExists;
                tetriInfoDict[kvp.Key] = tetriInfo;
            }

            // 更新tetriList
            tetriList = new List<OperationTableTetri>();
            foreach (var kvp in tetriInfoDict)
            {
                if (kvp.Value.isActive)
                {
                    tetriList.Add(new OperationTableTetri(kvp.Value.startPoint, kvp.Value.tetri));
                }
            }

            // 触发改变事件
            OnTableChanged?.Invoke();
            OnRowCleared?.Invoke(new RowClearedInfo(row, clearedBricks));

        }

        public void PrintBoard()
        {
            for (int i = 0; i < board.GetLength(0); i++)
            {
                string row = "";
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    if (board[i, j].Tile != emptyBrick.Tile)
                    {
                        row += board[i, j].Tile + " ";
                    }
                    else
                    {
                        row += "Empty ";
                    }
                }
                Debug.Log(row);
            }
        }

        public void PrintTetriBoard()
        {
            for (int i = 0; i < tetriBoard.GetLength(0); i++)
            {
                string row = "";
                for (int j = 0; j < tetriBoard.GetLength(1); j++)
                {
                    row += tetriBoard[i, j] + " ";
                }
                Debug.Log(row);
            }
        }

        public Brick[,] GetBoardData()
        {
            return board;
        }

        public bool PlaceTetri(Vector2Int position, Tetri tetri)
        {
            // 检查是否可以放置Tetri
            for (int i = 0; i < tetri.shape.GetLength(0); i++)
            {
                for (int j = 0; j < tetri.shape.GetLength(1); j++)
                {
                    if (tetri.shape[i, j] != 0)
                    {
                        int x = position.x + j; // 调整行列索引
                        int y = position.y - i ; // 调整行列索引

                        if (x < 0 || x >= board.GetLength(0) || y < 0 || y >= board.GetLength(1) 
                            || board[x, y].Tile != emptyBrick.Tile)
                        {
                            Debug.LogWarning("Cannot place Tetri at the specified position.");
                            return false;
                        }
                    }
                }
            }

            // 放置Tetri
            for (int i = 0; i < tetri.shape.GetLength(0); i++)
            {
                for (int j = 0; j < tetri.shape.GetLength(1); j++)
                {
                    if (tetri.shape[i, j] != 0)
                    {
                       
                        int x = position.x + j; // 调整行列索引
                        int y = position.y - i ; // 调整行列索引

                        board[x, y] = Instantiate(setedBrick); // 使用一个示例Brick对象来填充棋盘
                        tetriBoard[x, y] = tetriList.Count + 1; // 记录当前格子属于哪个Tetri
                    }
                }
            }

            // 更新TetriInfo
            tetriList.Add(new OperationTableTetri(position, tetri));
            tetriInfoDict[tetriList.Count] = new TetriInfo(tetriList.Count, position, tetri);

            // 触发事件
            OnTableChanged?.Invoke();
            return true;
        }
    }

    [Serializable]
    public struct OperationTableTetri {
        public Vector2Int startPoint; // 二维坐标作为起始点
        public Tetri tetri; // 对外部Tetri结构的引用

        public OperationTableTetri(Vector2Int startPoint, Tetri tetri)
        {
            this.startPoint = startPoint;
            this.tetri = tetri;
        }
    }

    [Serializable]
    public struct Tetri
    {   
        public int[,] shape; // tetri的形状

        public Tetri(int[,] shape)
        {
            this.shape = shape;
        }
    }

    [Serializable]
    public struct TetriInfo
    {
        public int index; // Tetri的索引
        public Vector2Int startPoint; // Tetri的起始点
        public Tetri tetri; // Tetri的形状
        public bool isActive; // Tetri是否仍然存在于棋盘上

        public TetriInfo(int index, Vector2Int startPoint, Tetri tetri)
        {
            this.index = index;
            this.startPoint = startPoint;
            this.tetri = tetri;
            isActive = true;
        }
    }

    [Serializable]
    public struct RowClearedInfo
    {
        public int row; // 被清除的行号
        public List<Brick> clearedBricks; // 被清除的方块信息列表

        public RowClearedInfo(int row, List<Brick> clearedBricks)
        {
            this.row = row;
            this.clearedBricks = clearedBricks;
        }
    }
}

