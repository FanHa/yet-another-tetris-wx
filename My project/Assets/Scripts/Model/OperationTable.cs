using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Model.Tetri;
using Unity.VisualScripting.Antlr3.Runtime.Tree;

namespace Model
{
    [CreateAssetMenu(fileName = "OperationTable", menuName = "ScriptableObjects/OperationTable", order = 1)]
    public class OperationTable : ScriptableObject
    {
        public event Action OnTableChanged;
        public event Action<RowClearedInfo> OnRowCleared; // 定义事件，参数为RowClearedInfo

        [SerializeField] private Tile emptyBrickTile; // 空砖块的Tile

        [SerializeField] private Serializable2DArray<TetriCell> board; // 棋盘
        [SerializeField] private TetriCellTypeResourceMapping spriteMapping; // TetriCellTypeSpriteMapping实例


        public void Init(int rows, int columns)
        {
            board = new Serializable2DArray<TetriCell>(rows, columns);

             // 初始化棋盘，将所有格子变为type为"Setable"的Brick
            for (int x = 0; x < rows; x++)
            {
                for (int y = 0; y < columns; y++)
                {
                    board[x, y] = new TetriCellEmpty(); // 使用一个实例Brick对象来填充棋盘
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
                    if (board[x, y] is TetriCellEmpty)
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
            List<TetriCell> clearedCells = new List<TetriCell>();
            for (int x = 0; x < board.GetLength(0); x++)
            {
                clearedCells.Add(board[x, row]);
                if (board[x, row] is not TetriCellEmpty)
                {
                    board[x, row] = new TetriCellEmpty();;
                }
                board[x, row] = new TetriCellEmpty(); // 使用一个示例Brick对象来填充棋盘
            }


            // 触发改变事件
            OnTableChanged?.Invoke();
            OnRowCleared?.Invoke(new RowClearedInfo(row, clearedCells));

        }

        public void PrintBoard()
        {
            for (int i = 0; i < board.GetLength(0); i++)
            {
                string row = "";
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    if (board[i, j] is not TetriCellEmpty)
                    {
                        row += board[i, j] + " ";
                    }
                    else
                    {
                        row += "Empty ";
                    }
                }
                Debug.Log(row);
            }
        }


        public Serializable2DArray<TetriCell> GetBoardData()
        {
            return board;
        }

        public bool PlaceTetri(Vector2Int position, Tetri.Tetri tetri)
        {
            // 检查是否可以放置Tetri
            for (int i = 0; i < tetri.Shape.GetLength(0); i++)
            {
                for (int j = 0; j < tetri.Shape.GetLength(1); j++)
                {
                    if (tetri.Shape[i, j] is not TetriCellEmpty)
                    {
                        int x = position.x + j; // 调整行列索引
                        int y = position.y - i ; // 调整行列索引

                        if (x < 0 || x >= board.GetLength(0) || y < 0 || y >= board.GetLength(1) 
                            || board[x, y] is not TetriCellEmpty)
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
                    TetriCell cell = tetri.Shape[i, j];
                    if (cell is not TetriCellEmpty)
                    {
                        // var tile = spriteMapping.GetTile(cell.GetType());
                        int x = position.x + j; // 调整行列索引
                        int y = position.y - i ; // 调整行列索引
                        board[x, y] = cell; // 使用一个示例Brick对象来填充棋盘
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
        public Tetri.Tetri tetri; // 对外部Tetri结构的引用

        public OperationTableTetri(Vector2Int startPoint, Tetri.Tetri tetri)
        {
            this.startPoint = startPoint;
            this.tetri = tetri;
        }
    }


    [Serializable]
    public struct RowClearedInfo
    {
        public int row; // 被清除的行号
        public List<TetriCell> clearedCells; // 被清除的方块信息列表

        public RowClearedInfo(int row, List<TetriCell> clearedCells)
        {
            this.row = row;
            this.clearedCells = clearedCells;
        }
    }
}

