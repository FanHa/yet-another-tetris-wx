using System;
using System.Collections.Generic;
using UnityEngine;
using Model.Tetri;

namespace Model
{
    [CreateAssetMenu(fileName = "OperationTable", menuName = "ScriptableObjects/OperationTable", order = 1)]
    public class OperationTable : ScriptableObject
    {
        public event Action OnTableChanged;

        [SerializeField] private Serializable2DArray<Cell> board; // 棋盘
        [SerializeField] private List<CellTypeReference> initialCells;


        private TetriCellFactory _tetriCellFactory = new TetriCellFactory();

        public void Init(int rows, int columns)
        {
            board = new Serializable2DArray<Cell>(rows, columns);

            for (int x = 0; x < rows; x++)
            {
                for (int y = 0; y < columns; y++)
                {
                    board[x, y] = new Empty(); 
                }
            }

            int targetRow = 4;
            int targetCol = 4;
            int initialCellIndex = 0;
            for (int c = 0; c < columns; c++)
            {
                if (c == targetCol) 
                {
                    board[targetRow, c] = _tetriCellFactory.CreateRandomCharacter();
                }
                else
                {
                    if (initialCellIndex < initialCells.Count)
                    {
                        board[targetRow, c] = initialCells[initialCellIndex].CreateInstance();
                        initialCellIndex++;
                    } else 
                    {
                        board[targetRow, c] = _tetriCellFactory.CreatePadding(); 

                    }
                }
            }

        }
        public Serializable2DArray<Cell> GetBoardData()
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
                    if (tetri.Shape[i, j] is not Empty)
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
                    Cell cell = tetri.Shape[i, j];
                    if (cell is not Empty)
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

        public bool PlaceCharacterInRandomRow(Cell characterCell)
        {
            List<int> validRows = new List<int>();
            List<int> characterRows = new List<int>();

            // 找到所有包含 Character 的行
            for (int x = 0; x < board.GetLength(0); x++)
            {
                bool hasCharacter = false;

                for (int y = 0; y < board.GetLength(1); y++)
                {
                    if (board[x, y] is Character)
                    {
                        hasCharacter = true;
                        break;
                    }
                }

                if (hasCharacter)
                {
                    characterRows.Add(x);
                }
            }

            // 根据包含 Character 的行，寻找上方或下方的空行
            foreach (int characterRow in characterRows)
            {
                // 检查上方的行
                if (characterRow > 0  && !characterRows.Contains(characterRow - 1))
                {
                    validRows.Add(characterRow - 1);
                }

                // 检查下方的行
                if (characterRow < board.GetLength(0) - 1  && !characterRows.Contains(characterRow + 1))
                {
                    validRows.Add(characterRow + 1);
                }
            }

            // 如果没有找到有效行，返回 false
            if (validRows.Count == 0)
            {
                Debug.LogWarning("No valid rows available to place the CharacterCell.");
                return false;
            }

            // 随机选择一行
            int randomRow = validRows[UnityEngine.Random.Range(0, validRows.Count)];

            // 设置 [randomRow, y] 为 CharacterCell
            for (int y = 0; y < board.GetLength(1); y++)
            {
                if (y == randomRow)
                {
                    board[randomRow, y] = characterCell;
                }
                else if (board[randomRow, y] is Empty)
                {
                    board[randomRow, y] = _tetriCellFactory.CreatePadding();
                }
            }

            // 触发事件
            OnTableChanged?.Invoke();
            return true;
        }

        public List<List<Cell>> GetFullRows()
        {
            List<List<Cell>> fullRows = new List<List<Cell>>();

            for (int x = 0; x < board.GetLength(0); x++)
            {
                bool isFullRow = true;
                bool containsCharacter = false;
                List<Cell> rowCells = new List<Cell>();

                for (int y = 0; y < board.GetLength(1); y++)
                {
                    Cell cell = board[x, y];

                    // 判断是否为空单元格
                    if (cell is Empty)
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

