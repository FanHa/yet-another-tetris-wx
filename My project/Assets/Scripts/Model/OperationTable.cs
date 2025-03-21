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

        [SerializeField] private Tile emptyBrickTile; // 空砖块的Tile

        [SerializeField] private Serializable2DArray<TetriCell> board; // 棋盘

        private TetriCellFactory _tetriCellFactory = new TetriCellFactory();

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

            // 随机选一行，使用TetriCellFactory的CreateBasicCell填充
            var random = new System.Random();
            int randomRow = random.Next(rows);
            for (int x = 0; x < columns; x++)
            {
                board[x, randomRow] = _tetriCellFactory.CreateBasicCell();
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
                        int x = position.x + i; // 调整行列索引
                        int y = position.y + j ; // 调整行列索引

                        if (x < 0 || x >= board.GetLength(0) || y < 0 || y >= board.GetLength(1) )
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
                        int x = position.x + i; // 调整行列索引
                        int y = position.y + j ; // 调整行列索引
                        board[x, y] = cell;
                    }
                }
            }


            // 触发事件
            OnTableChanged?.Invoke();
            return true;
        }

        public List<List<TetriCell>> GetFullRows()
        {
            List<List<TetriCell>> fullRows = new List<List<TetriCell>>();
            for (int y = 0; y < board.GetLength(1); y++)
            {
                bool isFullRow = true;
                List<TetriCell> rowCells = new List<TetriCell>();
                for (int x = 0; x < board.GetLength(0); x++)
                {
                    if (board[x, y] is TetriCellEmpty)
                    {
                        isFullRow = false;
                        break;
                    }
                    rowCells.Add(board[x, y]);
                }

                if (isFullRow)
                {
                    fullRows.Add(rowCells);
                }
            }
            return fullRows;
        }
    }
}

