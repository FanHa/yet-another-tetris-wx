using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Model
{
    [CreateAssetMenu(fileName = "OperationTable", menuName = "ScriptableObjects/OperationTable", order = 1)]
    public class OperationTable : ScriptableObject
    {
        public event Action OnTableChanged;
        public event Action<RowClearedInfo> OnRowCleared; // 定义事件，参数为RowClearedInfo

        [SerializeField] private Tile emptyBrickTile; // 空砖块的Tile
        [SerializeField] private Tile setedBrickTile; // 已放置砖块的Tile

        [SerializeField] private Serializable2DArray<Brick> board; // 棋盘


        public void Init(int rows, int columns)
        {
            board = new Serializable2DArray<Brick>(rows, columns);

             // 初始化棋盘，将所有格子变为type为"Setable"的Brick
            for (int x = 0; x < rows; x++)
            {
                for (int y = 0; y < columns; y++)
                {
                    board[x, y] = new Brick(emptyBrickTile); // 使用一个实例Brick对象来填充棋盘
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
                    if (board[x, y].Tile == emptyBrickTile)
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
                if (board[x, row] != null)
                {
                    board[x, row] = null;
                }
                board[x, row] = new Brick(emptyBrickTile); 
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
                    if (board[i, j].Tile != emptyBrickTile)
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


        public Serializable2DArray<Brick> GetBoardData()
        {
            return board;
        }

        public bool PlaceTetri(Vector2Int position, Tetri tetri)
        {
            // 检查是否可以放置Tetri
            for (int i = 0; i < tetri.Shape.GetLength(0); i++)
            {
                for (int j = 0; j < tetri.Shape.GetLength(1); j++)
                {
                    if (tetri.Shape[i, j] != 0)
                    {
                        int x = position.x + j; // 调整行列索引
                        int y = position.y - i ; // 调整行列索引

                        if (x < 0 || x >= board.GetLength(0) || y < 0 || y >= board.GetLength(1) 
                            || board[x, y].Tile != emptyBrickTile)
                        {
                            Debug.LogWarning("Cannot place Tetri at the specified position.");
                            return false;
                        }
                    }
                }
            }

            // 放置Tetri
            for (int i = 0; i < tetri.Shape.GetLength(0); i++)
            {
                for (int j = 0; j < tetri.Shape.GetLength(1); j++)
                {
                    if (tetri.Shape[i, j] != 0)
                    {
                       
                        int x = position.x + j; // 调整行列索引
                        int y = position.y - i ; // 调整行列索引

                        board[x, y] = new Brick(setedBrickTile); // 使用一个示例Brick对象来填充棋盘
                    }
                }
            }


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

