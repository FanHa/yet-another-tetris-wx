using System;
using System.Collections.Generic;
using UnityEngine;

namespace Model.Tetri
{

    [CreateAssetMenu(menuName = "Factory/TetriModelFactory")]
    public class TetriFactory : ScriptableObject
    {
        [SerializeField] private Model.Tetri.TetriCellFactory tetriCellModelFactory;

        private readonly Dictionary<string, List<(int, int)>> shapeDefinitions = new()
        {
            { "T", new List<(int, int)> { (1, 0), (1, 1), (1, 2), (0, 1) } },
            { "I", new List<(int, int)> { (0, 1), (1, 1), (2, 1), (3, 1) } },
            { "O", new List<(int, int)> { (0, 0), (0, 1), (1, 0), (1, 1) } },
            { "L", new List<(int, int)> { (0, 1), (1, 1), (2, 1), (2, 2) } },
            { "J", new List<(int, int)> { (0, 1), (1, 1), (2, 1), (2, 0) } },
            { "S", new List<(int, int)> { (1, 0), (1, 1), (2, 1), (2, 2) } },
            { "Z", new List<(int, int)> { (1, 1), (1, 2), (2, 0), (2, 1) } }
        };


        public Tetri CreateRandomShapeWithCell(CellTypeId cellType)
        {
            // 1. 随机选择一个形状
            var shapeKeys = new List<string> { "T", "I", "L", "J", "S", "Z" };
            var randomKey = shapeKeys[UnityEngine.Random.Range(0, shapeKeys.Count)];
            var positions = shapeDefinitions[randomKey];

            // 2. 创建 Tetri
            Tetri tetri = new Tetri(Tetri.TetriType.Normal);

            // 3. 随机选择一个格子用于填充目标 Cell
            int specialIndex = UnityEngine.Random.Range(0, positions.Count);

            for (int i = 0; i < positions.Count; i++)
            {
                var (row, col) = positions[i];
                if (i == specialIndex)
                {
                    // 用指定类型创建 Cell
                    Cell cell = tetriCellModelFactory.CreateCell(cellType);
                    tetri.SetCell(row, col, cell);
                }
                else
                {
                    // 其余填充 Padding
                    tetri.SetCell(row, col, tetriCellModelFactory.CreatePadding());
                }
            }

            return tetri;
        }

        public Tetri CreateCharacterTetri(CharacterTypeId characterTypeId)
        {
            var positions = shapeDefinitions["O"];
            Tetri tetri = new Tetri(Tetri.TetriType.Character);

            // TODO 展示只在第一个位置填充角色cell，其余为Padding
            for (int i = 0; i < positions.Count; i++)
            {
                var (row, col) = positions[i];
                if (i == 0)
                    tetri.SetCell(row, col, tetriCellModelFactory.CreateCharacterCell(characterTypeId));
                else
                    tetri.SetCell(row, col, tetriCellModelFactory.CreatePadding());
            }
            return tetri;
        }

    }
}