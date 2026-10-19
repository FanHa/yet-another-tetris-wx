using System;
using System.Collections.Generic;
using Model.Resonance;
using UnityEngine;

namespace Model.Tetri
{

    [CreateAssetMenu(menuName = "Game Data/Bootstrap/Factories/Tetri Model Factory")]
    public class TetriFactory : ScriptableObject
    {
        [SerializeField] private Model.Tetri.TetriCellFactory tetriCellModelFactory;
        [SerializeField] private CharacterFactory characterFactory;

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


#if UNITY_EDITOR
        private void OnValidate()
        {
            if (tetriCellModelFactory == null)
            {
                Debug.LogError($"[{nameof(TetriFactory)}] Tetri cell factory is missing.", this);
            }

            if (characterFactory == null)
            {
                Debug.LogError($"[{nameof(TetriFactory)}] Character factory is missing.", this);
            }

            foreach (var entry in shapeDefinitions)
            {
                if (entry.Value.Count != 4)
                {
                    Debug.LogError($"[{nameof(TetriFactory)}] Shape '{entry.Key}' must contain exactly four positions.", this);
                }

                var uniquePositions = new HashSet<(int, int)>();
                foreach (var position in entry.Value)
                {
                    var (row, column) = position;
                    if (row < 0 || row >= Tetri.TetriGridSize || column < 0 || column >= Tetri.TetriGridSize)
                    {
                        Debug.LogError($"[{nameof(TetriFactory)}] Shape '{entry.Key}' position {position} is outside the canvas.", this);
                    }

                    if (!uniquePositions.Add(position))
                    {
                        Debug.LogError($"[{nameof(TetriFactory)}] Shape '{entry.Key}' contains duplicate position {position}.", this);
                    }
                }
            }
        }
#endif

        public CellBasedTetri CreateRandomCellBasedTetri(CellDefinition definition)
        {
            return CreateRandomCellBasedTetri(tetriCellModelFactory.CreateCell(definition));
        }

        private CellBasedTetri CreateRandomCellBasedTetri(Cell specialCell)
        {
            // 1. 随机选择一个形状
            var shapeKeys = new List<string> { "T", "I", "L", "J", "S", "Z" };
            var randomKey = shapeKeys[UnityEngine.Random.Range(0, shapeKeys.Count)];
            var positions = shapeDefinitions[randomKey];

            var cells = new Dictionary<Vector2Int, Cell>();

            // 3. 随机选择一个格子用于填充目标 Cell
            int specialIndex = UnityEngine.Random.Range(0, positions.Count);
            AffinityType targetAffinity = specialCell.Affinity;
            ResonanceType targetResonance = specialCell.Resonance;

            for (int i = 0; i < positions.Count; i++)
            {
                var (row, col) = positions[i];
                if (i == specialIndex)
                {
                    cells.Add(new Vector2Int(row, col), specialCell);
                }
                else
                {
                    Cell fillerCell = tetriCellModelFactory.CreateFillerCell();
                    fillerCell.Affinity = targetAffinity;
                    fillerCell.Resonance = targetResonance;
                    cells.Add(new Vector2Int(row, col), fillerCell);
                }
            }

            var tetri = new CellBasedTetri(cells, specialCell);

            // 随机旋转 0~3 次，固定初始朝向
            int rotations = UnityEngine.Random.Range(0, 4);
            for (int r = 0; r < rotations; r++)
                tetri.Rotate();

            return tetri;
        }

        public CharacterTetri CreateCharacterTetri(CharacterDefinition definition)
        {
            return new CharacterTetri(characterFactory.CreateCharacter(definition));
        }

        public Tetri Clone(Tetri original)
        {
            return original switch
            {
                CellBasedTetri cellBased => CloneCellBasedTetri(cellBased),
                CharacterTetri character => new CharacterTetri(
                    characterFactory.Clone(character.Character), character.UpgradedTimes),
                _ => throw new ArgumentException("Unsupported Tetri type.", nameof(original))
            };
        }

        private CellBasedTetri CloneCellBasedTetri(CellBasedTetri original)
        {
            var clonedCells = new Dictionary<Vector2Int, Cell>();
            var clonedMainCell = tetriCellModelFactory.Clone(original.MainCell);
            foreach (var entry in original.Cells)
            {
                var clonedCell = ReferenceEquals(entry.Value, original.MainCell)
                    ? clonedMainCell
                    : tetriCellModelFactory.Clone(entry.Value);
                clonedCells.Add(entry.Key, clonedCell);
            }

            return new CellBasedTetri(clonedCells, clonedMainCell, original.UpgradedTimes);
        }

    }
}