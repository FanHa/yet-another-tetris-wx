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

            // 确定 Character 的位置
            int targetRow = rows / 2; // 中间行
            int targetCol = columns / 2; // 中间列
            board[targetRow, targetCol] = _tetriCellFactory.CreateRandomCharacter();

            int initialCellIndex = 0;
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    // 跳过 Character 自己的位置
                    if (dx == 0 && dy == 0) continue;

                    int x = targetRow + dx;
                    int y = targetCol + dy;

                    // 检查是否在棋盘范围内
                    if (x >= 0 && x < rows && y >= 0 && y < columns)
                    {
                        if (initialCellIndex < initialCells.Count)
                        {
                            board[x, y] = initialCells[initialCellIndex].CreateInstance();
                            initialCellIndex++;
                        }
                        else
                        {
                            board[x, y] = _tetriCellFactory.CreatePadding();
                        }
                    }
                }
            }

        }
        public Serializable2DArray<Cell> GetBoardData()
        {
            return board;
        }

        /// <summary>
        /// 获取当前 table 中各种角色的数量
        /// </summary>
        /// <returns>字典，键为角色类型，值为数量</returns>
        public Dictionary<Type, int> GetCharacterCounts()
        {
            Dictionary<Type, int> characterCounts = new Dictionary<Type, int>();

            for (int x = 0; x < board.GetLength(0); x++)
            {
                for (int y = 0; y < board.GetLength(1); y++)
                {
                    if (board[x, y] is Character character)
                    {
                        Type characterType = character.GetType();
                        if (characterCounts.ContainsKey(characterType))
                        {
                            characterCounts[characterType]++;
                        }
                        else
                        {
                            characterCounts[characterType] = 1;
                        }
                    }
                }
            }

            return characterCounts;
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

        public List<List<Cell>> GetCharacterCellGroups()
        {
            List<List<Cell>> cellGroups = new List<List<Cell>>();

            for (int x = 0; x < board.GetLength(0); x++)
            {
                for (int y = 0; y < board.GetLength(1); y++)
                {
                    if (board[x, y] is Character)
                    {
                        List<Cell> group = new List<Cell>
                        {
                            // 添加当前 CharacterCell
                            board[x, y]
                        };

                        // 获取周围一圈的非 CharacterCell
                        for (int dx = -1; dx <= 1; dx++)
                        {
                            for (int dy = -1; dy <= 1; dy++)
                            {
                                if (dx == 0 && dy == 0) 
                                    continue; // 跳过中心点
                                int nx = x + dx;
                                int ny = y + dy;

                                if (nx >= 0 && nx < board.GetLength(0) && ny >= 0 && ny < board.GetLength(1))
                                {
                                    Cell neighborCell = board[nx, ny];
                                    if (!(neighborCell is Character) && !(neighborCell is Empty))
                                    {
                                        group.Add(neighborCell);
                                    }
                                }
                            }
                        }

                        // 将打包的组添加到结果中
                        cellGroups.Add(group);
                    }
                }
            }

            return cellGroups;
        }
    }
}

