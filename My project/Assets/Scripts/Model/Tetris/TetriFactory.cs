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
            { "O", new List<(int, int)> { (1, 1), (1, 2), (2, 1), (2, 2) } },
            { "L", new List<(int, int)> { (0, 1), (1, 1), (2, 1), (2, 2) } },
            { "J", new List<(int, int)> { (0, 1), (1, 1), (2, 1), (2, 0) } },
            { "S", new List<(int, int)> { (1, 0), (1, 1), (2, 1), (2, 2) } },
            { "Z", new List<(int, int)> { (1, 1), (1, 2), (2, 0), (2, 1) } }
        };


        public Tetri CreateRandomShapeWithCell(string cellId)
        {
            if (string.IsNullOrWhiteSpace(cellId))
                throw new ArgumentException("Cell id is null or empty.", nameof(cellId));

            // 1. 随机选择一个形状
            var shapeKeys = new List<string> { "T", "I", "L", "J", "S", "Z" };
            var randomKey = shapeKeys[UnityEngine.Random.Range(0, shapeKeys.Count)];
            var positions = shapeDefinitions[randomKey];

            // 2. 创建 Tetri
            Tetri tetri = new Tetri(Tetri.TetriType.Normal);

            // 3. 随机选择一个格子用于填充目标 Cell
            int specialIndex = UnityEngine.Random.Range(0, positions.Count);
            Cell specialCell = tetriCellModelFactory.CreateCell(cellId);
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
                    Cell padding = tetriCellModelFactory.CreatePadding();
                    padding.Affinity = targetAffinity; // 设置为目标 Cell 的 Affinity
                    tetri.SetCell(row, col, padding);
                }
            }

            tetri.SetMainCell(specialCell);

            // 随机旋转 0~3 次，固定初始朝向
            int rotations = UnityEngine.Random.Range(0, 4);
            for (int r = 0; r < rotations; r++)
                tetri.Rotate();

            return tetri;
        }

        public Tetri CreateRandomShapeWithCell(CellDefinition definition)
        {
            return CreateRandomShapeWithCell(definition.Id);
        }


        public Tetri CreateCharacterTetri(string characterDefinitionId)
        {
            if (string.IsNullOrWhiteSpace(characterDefinitionId))
                throw new ArgumentException("Character definition id is null or empty.", nameof(characterDefinitionId));

            var definition = tetriCellModelFactory.CreateCharacterCell(characterDefinitionId).DefinitionData;
            return CreateCharacterTetri(definition);
        }

        private Tetri CreateCharacterTetri(CharacterDefinition definition)
        {
            if (definition == null)
                throw new ArgumentNullException(nameof(definition));

            var positions = shapeDefinitions["O"];
            Tetri tetri = new Tetri(Tetri.TetriType.Character);

            Cell mainCharacterCell = null;
            for (int i = 0; i < positions.Count; i++)
            {
                var (row, col) = positions[i];
                if (i == 0)
                {
                    mainCharacterCell = tetriCellModelFactory.CreateCharacterCell(definition);
                    tetri.SetCell(row, col, mainCharacterCell);
                }
                else
                {
                    tetri.SetCell(row, col, tetriCellModelFactory.CreatePadding());
                }
            }

            tetri.SetMainCell(mainCharacterCell);
            return tetri;
        }

        public Tetri Clone(Tetri original)
        {
            Tetri clone = new Tetri(original.Type);
            var shape = original.Shape;
            int rows = shape.GetLength(0);
            int cols = shape.GetLength(1);

            Cell clonedMainCell = null;
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    var clonedCell = tetriCellModelFactory.Clone(shape[i, j]);
                    clone.SetCell(i, j, clonedCell);

                    if (ReferenceEquals(shape[i, j], original.MainCell))
                    {
                        clonedMainCell = clonedCell;
                    }
                }
            }

            clone.SetMainCell(clonedMainCell);
            clone.UpgradedTimes = original.UpgradedTimes;
            return clone;
        }

    }
}