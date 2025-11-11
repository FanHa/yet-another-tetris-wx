using System;
using System.Collections.Generic;
using Model.Tetri;
using UnityEngine;

namespace Model
{
    [CreateAssetMenu(fileName = "OperationTableModel", menuName = "Tetris/OperationTableModel")]
    public class OperationTableModel : ScriptableObject
    {
        public int Width;
        public int Height;
        public event Action OnChanged;

        [SerializeField]

        private Dictionary<Model.Tetri.Tetri, Vector2Int> placedMap = new();
        public IReadOnlyDictionary<Model.Tetri.Tetri, Vector2Int> PlacedMap => placedMap;

        // private Cell[,] occupiedCellGrid;
        private OccupiedSlot[,] occupiedGrid;


        private void Awake()
        {
            occupiedGrid = new OccupiedSlot[Width, Height];
        }

        /// <summary>
        /// 尝试放置一个 Tetri 到指定位置
        /// </summary>
        /// <param name="tetri">要放置的 Tetri</param>
        /// <param name="position">左上角在表格上的格子坐标</param>
        /// <returns>是否放置成功</returns>
        public bool TryPlaceTetri(Model.Tetri.Tetri tetri, Vector2Int position)
        {
            if (tetri == null) return false;
            if (placedMap.ContainsKey(tetri)) return false;

            List<Vector2Int> rels = tetri.GetOccupiedPositions();
            // 边界 & 占用检测
            foreach (Vector2Int r in rels)
            {
                Vector2Int g = position + r;
                if (g.x < 0 || g.x >= Width || g.y < 0 || g.y >= Height) return false;
                if (occupiedGrid[g.x, g.y] != null) return false;
            }
            tetri.OnDataChanged += HandleTetriChanged;

            // 写入
            foreach (Vector2Int r in rels)
            {
                Vector2Int g = position + r;
                Model.Tetri.Cell cell = tetri.Shape[r.x, r.y];
                if (cell == null) continue;

                occupiedGrid[g.x, g.y] = new OccupiedSlot
                {
                    Cell = cell,
                    OwnerTetri = tetri,
                    LocalInTetri = r
                };
            }

            placedMap[tetri] = position;
            OnChanged?.Invoke();
            return true;
        }

        public void Clear()
        {
            foreach (KeyValuePair<Tetri.Tetri, Vector2Int> kv in placedMap)
            {
                kv.Key.OnDataChanged -= HandleTetriChanged;
            }
            placedMap.Clear();
            occupiedGrid = new OccupiedSlot[Width, Height];
            OnChanged?.Invoke();
        }

        public void RemoveTetri(Model.Tetri.Tetri tetri)
        {
            if (tetri == null) return;
            if (!placedMap.TryGetValue(tetri, out var topLeft)) return;
            tetri.OnDataChanged -= HandleTetriChanged;
            foreach (var r in tetri.GetOccupiedPositions())
            {
                var g = topLeft + r;
                if (g.x < 0 || g.x >= Width || g.y < 0 || g.y >= Height) continue;
                occupiedGrid[g.x, g.y] = null;
            }
            placedMap.Remove(tetri);
            OnChanged?.Invoke();

        }

        private void HandleTetriChanged()
        {
            // Tetri 变动时，通知外部刷新
            OnChanged?.Invoke();
        }

        public List<CharacterInfluence> GetCharacterInfluences()
        {
            var result = new List<CharacterInfluence>();
            int width = occupiedGrid.GetLength(0);
            int height = occupiedGrid.GetLength(1);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Model.OccupiedSlot slot = occupiedGrid[x, y];
                    if (slot?.Cell is Model.Tetri.Character character)
                    {
                        var influencedCells = new List<Cell>();
                        var offsets = character.GetInfluenceOffsets();

                        foreach (var offset in offsets)
                        {
                            int nx = x + offset.x;
                            int ny = y + offset.y;
                            if (nx >= 0 && nx < width && ny >= 0 && ny < height)
                            {
                                var neighbor = occupiedGrid[nx, ny]?.Cell;
                                if (neighbor != null && !influencedCells.Contains(neighbor))
                                {
                                    influencedCells.Add(neighbor);
                                }
                            }
                        }
                        var characterInfluence = new CharacterInfluence(character, influencedCells, slot.OwnerTetri);
                        result.Add(characterInfluence);
                    }
                }
            }

            return result;
        }

        public CharacterInfluence GetCharacterInfluenceByTetri(Model.Tetri.Tetri tetri)
        {
            if (tetri == null) return null;
            if (!placedMap.TryGetValue(tetri, out var topLeft)) return null;

            Model.Tetri.Character main = tetri.GetMainCell() as Model.Tetri.Character;
            if (main == null) return null;

            // 找主角色在 Tetri 内的局部坐标
            Vector2Int? local = null;
            var shape = tetri.Shape;
            for (int i = 0; i < shape.GetLength(0) && !local.HasValue; i++)
                for (int j = 0; j < shape.GetLength(1) && !local.HasValue; j++)
                    if (ReferenceEquals(shape[i, j], main))
                        local = new Vector2Int(i, j);
            if (!local.HasValue) return null;

            var basePos = topLeft + local.Value;
            int w = occupiedGrid.GetLength(0);
            int h = occupiedGrid.GetLength(1);

            var influenced = new List<Cell>();
            foreach (var o in main.GetInfluenceOffsets())
            {
                int nx = basePos.x + o.x;
                int ny = basePos.y + o.y;
                if (nx >= 0 && nx < w && ny >= 0 && ny < h)
                {
                    var nCell = occupiedGrid[nx, ny]?.Cell;
                    if (nCell != null && !influenced.Contains(nCell))
                        influenced.Add(nCell);
                }
            }

            return new CharacterInfluence(main, influenced, tetri);

        }

    }
    
    [Serializable]
    public class OccupiedSlot
    {
        public Cell Cell;
        public Model.Tetri.Tetri OwnerTetri;
        public Vector2Int LocalInTetri;   // 该格在 Tetri 内的局部坐标
    }
}