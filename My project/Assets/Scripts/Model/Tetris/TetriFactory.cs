using System;
using System.Collections.Generic;
using Unity.VisualScripting;
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
            { "O", new List<(int, int)> { (1, 1), (1, 2), (2, 1), (2, 2) } },
            { "L", new List<(int, int)> { (0, 1), (1, 1), (2, 1), (2, 2) } },
            { "J", new List<(int, int)> { (0, 1), (1, 1), (2, 1), (2, 0) } },
            { "S", new List<(int, int)> { (1, 0), (1, 1), (2, 1), (2, 2) } },
            { "Z", new List<(int, int)> { (1, 1), (1, 2), (2, 0), (2, 1) } }
        };


        public Tetri CreateRandomShapeWithCell(CellTypeId cellTypeId)
        {
            // 1. 随机选择一个形状
            var shapeKeys = new List<string> { "T", "I", "L", "J", "S", "Z" };
            var randomKey = shapeKeys[UnityEngine.Random.Range(0, shapeKeys.Count)];
            var positions = shapeDefinitions[randomKey];

            // 2. 创建 Tetri
            Tetri tetri = new Tetri(Tetri.TetriType.Normal);

            // 3. 随机选择一个格子用于填充目标 Cell
            int specialIndex = UnityEngine.Random.Range(0, positions.Count);
            Cell specialCell = tetriCellModelFactory.CreateCell(cellTypeId);
            AffinityType targetAffinity = specialCell.Affinity;

            for (int i = 0; i < positions.Count; i++)
            {
                var (row, col) = positions[i];
                if (i == specialIndex)
                {
                    tetri.SetCell(row, col, specialCell);
                }
                else
                {
                    Padding padding = tetriCellModelFactory.CreatePadding();
                    padding.Affinity = targetAffinity; // 设置为目标 Cell 的 Affinity
                    tetri.SetCell(row, col, padding);
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

        public Tetri Clone(Tetri original)
        {
            Tetri clone = new Tetri(original.Type);
            var shape = original.Shape;
            int rows = shape.GetLength(0);
            int cols = shape.GetLength(1);

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    clone.SetCell(i, j, tetriCellModelFactory.Clone(shape[i, j]));
                }
            }
            clone.UpgradedTimes = original.UpgradedTimes;
            return clone;
        }

    }
}