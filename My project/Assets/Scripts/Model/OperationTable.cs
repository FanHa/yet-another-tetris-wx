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

            for (int x = 0; x < rows; x++)
            {
                for (int y = 0; y < columns; y++)
                {
                    board[x, y] = new TetriCellEmpty(); 
                }
            }

            int targetRow = 4;
            int targetCol = 4;
            for (int c = 0; c < columns; c++)
            {
                if (c == targetCol) 
                {
                    board[targetRow, c] = _tetriCellFactory.CreateRandomCharacter();
                }
                else
                {
                    board[targetRow, c] = _tetriCellFactory.CreatePadding(); 
                }
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

                        // 检查是否会覆盖 TetriCellCharacter
                        if (board[x, y] is Character)
                        {
                            Debug.LogWarning("Cannot place Tetri at the specified position: Overlaps with TetriCellCharacter.");
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

            for (int x = 0; x < board.GetLength(0); x++)
            {
                bool isFullRow = true;
                bool containsCharacter = false;
                List<TetriCell> rowCells = new List<TetriCell>();

                for (int y = 0; y < board.GetLength(1); y++)
                {
                    TetriCell cell = board[x, y];

                    // 判断是否为空单元格
                    if (cell is TetriCellEmpty)
                    {
                        isFullRow = false;
                        break;
                    }

                    // 判断是否包含 TetriCellCharacter
                    if (cell is Character)
                    {
                        containsCharacter = true;
                    }

                    rowCells.Add(cell);
                }

                // 如果行已满且包含至少一个 TetriCellCharacter，则加入结果
                if (isFullRow && containsCharacter)
                {
                    fullRows.Add(rowCells);
                }
            }

            return fullRows;
        }
    }
}

